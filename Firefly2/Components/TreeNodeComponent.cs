using Firefly2.Facilities;
using Firefly2.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class TreeNodeComponent : Component, ITakesMessage<ComponentCollectionChanged>
	{
		public class QueryResults<TQuery, TAnswer, TComp> where TComp : Component
		{
			private IList<TreeNodeComponent> children;
			private TQuery query;
			private Dictionary<int, TAnswer> results;

			private TAnswer GetAtIndex(int index)
			{
				if (results.ContainsKey(index)) return results[index];
				else
				{
					var comp = children[index].Host.GetComponent<TComp>();
					if (comp != null)
					{
						var func = comp as IAnswersMessage<TQuery, TAnswer>;
						return results[index] = func != null ? func.AnswerMessage(query) : default(TAnswer);
					}
					else
					{
						return results[index] = default(TAnswer);
					}
				}
			}

			public QueryResults(IList<TreeNodeComponent> children, TQuery query)
			{
				this.children = children;
				this.query = query;
				results = new Dictionary<int, TAnswer>();
			}

			public List<TAnswer> ToList()
			{
				var list = new List<TAnswer>();
				for (int i = 0; i < children.Count; ++i)
				{
					list.Add(GetAtIndex(i));
				}
				return list;
			}

			public bool Any(Func<TAnswer, bool> func)
			{
				for (int i = 0; i < children.Count; ++i)
				{
					if (func(GetAtIndex(i))) return true;
				}
				return false;
			}

			public bool All(Func<TAnswer, bool> func)
			{
				for (int i = 0; i < children.Count; ++i)
				{
					if (!func(GetAtIndex(i))) return false;
				}
				return true;
			}
		}

		public enum SendRange
		{
			Parent,
			ImmediateChildrenOnly,
			WholeTree
		}

		public enum QueryRange
		{
			Parent,
			ImmediateChildrenOnly,
			/// <summary>
			/// Propagates down the tree, skipping nodes that don't have the target component, 
			/// but stops at nodes do
			/// </summary>
			StopAtReceivers,
			/// <summary>
			/// Propagates down the tree, skipping nodes that don't have the target component
			/// </summary>
			WholeTree
		}

		public ObservableCollection<TreeNodeComponent> Children;
		public TreeNodeComponent Parent;

		private LinkedList<WeakReference<Uplink>> uplinks;
		private LinkedList<WeakReference<Downlink>> downlinks;

		public TreeNodeComponent() : this(null) { }

		public TreeNodeComponent(TreeNodeComponent parent)
			: base()
		{
			uplinks = new LinkedList<WeakReference<Uplink>>();
			downlinks = new LinkedList<WeakReference<Downlink>>();
			Parent = parent;
			Children = new ObservableCollection<TreeNodeComponent>();

			Children.CollectionChanged += delegate(object target, NotifyCollectionChangedEventArgs args)
			{
				var downlinkValues = new List<Downlink>();
				downlinks.ToList().ForEach(refer =>
				{
					var downlink = refer.GetTarget();
					if (downlink != null) downlinkValues.Add(downlink);
					else downlinks.Remove(refer);
				});
				uplinks.RemoveWhere(refer => !refer.IsAlive());

				if (args.NewItems != null)
				{
					foreach (var downlink in downlinkValues)
					{
						downlink.CallGenericRelink(this);
					}
					foreach (TreeNodeComponent child in args.NewItems)
					{
						if (child.Parent != null) throw new Exception("Child already has a parent");
						child.Parent = this;

						foreach (var refer in child.uplinks)
						{
							Uplink uplink = refer.GetTarget();
							if (uplink != null) uplink.CallGenericRelink(child);
						}

						child.Host.SendMessage(NewParent.Instance);
						Host.SendMessage(new NewChild(child));
					}
				}
				if (args.OldItems != null)
				{
					foreach (TreeNodeComponent child in args.OldItems)
					{
						foreach (var refer in child.uplinks)
						{
							Uplink uplink = refer.GetTarget();
							if (uplink == null) continue;
							uplink.CastAndSetComponent(null);
							RemoveUplink(uplink);
						}
						foreach (var downlink in downlinkValues)
						{
							if (child.downlinks.RemoveWhere(refer => refer.GetTarget() == downlink)) child.RemoveDownlink(downlink);
							else downlink.RemoveMatchingComponent(child);
						}
						child.Parent = null;
						child.Host.SendMessage(RemovedFromParent.Instance);
					}
				}
			};
		}

		public void AddChild(Entity entity)
		{
			if (entity.GetComponent<TreeNodeComponent>() == null)
			{
				throw new Exception("The entity you are adding must have a TreeNodeComponent attached");
			}
			Children.Add(entity.GetComponent<TreeNodeComponent>());
		}

		public void RemoveChild(Entity entity)
		{
			Children.Remove(entity.GetComponent<TreeNodeComponent>());
		}

		public Uplink<T> CreateUplink<T>()
			where T : Component
		{
			var link = new Uplink<T>();
			uplinks.AddLast(new WeakReference<Uplink>(link));
			RestoreUplink(link);
			return link;
		}

		public void RestoreUplink<T>(Uplink<T> link)
			where T : Component
		{
			var current = Parent;
			while (current != null)
			{
				var comp = current.Host.GetComponent<T>();
				if (comp != null)
				{
					link.CastAndSetComponent(comp);
					break;
				}
				current.uplinks.AddLast(new WeakReference<Uplink>(link));
				current = current.Parent;
			}
		}

		public void RemoveUplink(Uplink link)
		{
			var current = this;
			while (current.uplinks.RemoveWhere(refer => refer.GetTarget() == link))
			{
				current = current.Parent;
				if (current == null) break;
			}
		}

		public Downlink<T> CreateDownlink<T>()
			where T : Component
		{
			var link = new Downlink<T>();
			downlinks.AddLast(new WeakReference<Downlink>(link));
			RestoreDownlink(link);
			return link;
		}

		public void RestoreDownlink<T>(Downlink<T> link)
			where T : Component
		{
			BFS(node =>
			{
				if (node.downlinks.Any(refer => refer.GetTarget() == link)) return false;

				var comp = node.Host.GetComponent<T>();
				if (comp != null)
				{
					link.CastAndAddComponent(comp);
					return false;
				}
				node.downlinks.AddLast(new WeakReference<Downlink>(link));
				return true;
			});
		}

		public void RemoveDownlink(Downlink link)
		{
			BFS(node =>
			{
				if (downlinks.RemoveWhere(refer => refer.GetTarget() == link))
				{
					return true;
				}
				link.RemoveMatchingComponent(node);
				return false;
			});
		}

		/// <summary>
		/// Propagates message down the tree with variable range
		/// </summary>
		/// <typeparam name="TMessage"></typeparam>
		/// <param name="message"></param>
		/// <param name="range">Specifies which nodes the message should be sent to</param>
		public void Send<TMessage>(TMessage message, SendRange range)
		{
			if (range == SendRange.Parent && Parent != null) Parent.Host.SendMessage(message);
			BFS(node =>
			{
				node.Host.SendMessage(message);
				return range == SendRange.WholeTree;
			});
		}

		/// <summary>
		/// Propagates message down the tree, stopping at nodes that contain the specified component
		/// </summary>
		/// <typeparam name="TMessage"></typeparam>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="message"></param>
		public void Send<TMessage, TComponent>(TMessage message)
			where TComponent : Component
		{
			BFS(node =>
			{
				if (node.Host.GetComponent<TComponent>() != null)
				{
					node.Host.SendMessage(message);
					return false;
				}
				return true;
			});
		}

		private static List<TreeNodeComponent> empty = new List<TreeNodeComponent>();
		public QueryResults<TQuery, TAnswer, TComp> Query<TQuery, TAnswer, TComp>(TQuery query, QueryRange range)
			where TComp : Component
		{
			if (range == QueryRange.Parent)
			{
				if (Parent != null) return new QueryResults<TQuery, TAnswer, TComp>(new List<TreeNodeComponent>() { Parent }, query);
				else return new QueryResults<TQuery, TAnswer, TComp>(empty, query);
			}
			var list = new List<TreeNodeComponent>();
			BFS(node =>
			{
				bool isTarget = node.Host.GetComponent<TComp>() != null;
				if (isTarget)
				{
					list.Add(node);
					if (range == QueryRange.WholeTree) return true;
					return false;
				}
				else
				{
					if (range == QueryRange.ImmediateChildrenOnly) return false;
					return true;
				}
			});
			return new QueryResults<TQuery, TAnswer, TComp>(list, query);
		}

		private void BFS(Func<TreeNodeComponent, bool> func)
		{
			var queue = new Queue<TreeNodeComponent>();
			foreach (var child in Children) queue.Enqueue(child);
			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				if (func(node)) foreach (var child in node.Children) queue.Enqueue(child);
			}
		}

		public void TakeMessage(ComponentCollectionChanged msg)
		{
			uplinks.RemoveWhere(refer => !refer.IsAlive());
			downlinks.RemoveWhere(refer => !refer.IsAlive());

			if (msg.Type == ComponentCollectionChanged.ChangeType.Add)
			{
				var ulinks = uplinks
					.Select(refer => refer.GetTarget())
					.Where(link => link != null && link.ContainsComponentsLike(msg.Target));
				if (ulinks.Count() > 0)
				{
					if (Parent != null) Parent.RemoveUplink(ulinks.First());
					ulinks.First().CastAndSetComponent(msg.Target);
				}
				var dlinks = downlinks
					.Select(refer => refer.GetTarget())
					.Where(link => link != null && link.ContainsComponentsLike(msg.Target));
				if (dlinks.Count() > 0)
				{
					foreach (var child in Children) child.RemoveDownlink(dlinks.First());
					dlinks.First().CastAndAddComponent(msg.Target);
				}
			}
			else if (msg.Type == ComponentCollectionChanged.ChangeType.Remove)
			{
				foreach (var child in Children)
				{
					foreach (var refer in child.uplinks)
					{
						Uplink uplink = refer.GetTarget();
						if (uplink == null) continue;
						if (uplink.ContainsComponentsLike(msg.Target))
						{
							uplink.CastAndSetComponent(null);
							uplink.CallGenericRelink(child);
						}
					}
				}
				if (Parent != null)
				{
					foreach (var refer in Parent.downlinks)
					{
						Downlink downlink = refer.GetTarget();
						if (downlink == null) continue;
						if (downlink.ContainsComponentsLike(msg.Target))
						{
							downlink.CastAndRemoveComponent(msg.Target);
							downlink.CallGenericRelink(Parent);
						}
					}
				}
			}
		}
	}
}
