using Firefly2.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class TreeNodeComponent : Component
	{
		public class QueryResults<Query, Answer, Comp> where Comp : Component
		{
			private ObservableCollection<TreeNodeComponent> children;
			private Query query;
			private Dictionary<int, Answer> results;

			private Answer GetAtIndex(int index)
			{
				if (results.ContainsKey(index)) return results[index];
				else
				{
					var comp = children[index].Host.GetComponent<Comp>();
					if (comp != null)
					{
						var func = comp as IMessageResponder<Query, Answer>;
						return results[index] = func != null ? func.HandleMessage(query) : default(Answer);
					}
					else
					{
						return results[index] = default(Answer);
					}
				}
			}

			public QueryResults(ObservableCollection<TreeNodeComponent> children, Query query)
			{
				this.children = children;
				this.query = query;
				results = new Dictionary<int, Answer>();
			}

			public List<Answer> ToList()
			{
				var list = new List<Answer>();
				for (int i = 0; i < children.Count; ++i)
				{
					list.Add(GetAtIndex(i));
				}
				return list;
			}

			public bool Any(Func<Answer, bool> func)
			{
				for (int i = 0; i < children.Count; ++i)
				{
					if (func(GetAtIndex(i))) return true;
				}
				return false;
			}

			public bool All(Func<Answer, bool> func)
			{
				for (int i = 0; i < children.Count; ++i)
				{
					if (!func(GetAtIndex(i))) return false;
				}
				return true;
			}
		}

		public ObservableCollection<TreeNodeComponent> Children;
		public TreeNodeComponent Parent;

		public TreeNodeComponent() : this(null) { }

		public TreeNodeComponent(TreeNodeComponent parent)
			: base()
		{
			Parent = parent;
			Children = new ObservableCollection<TreeNodeComponent>();
		}

		public QueryResults<Query, Answer, Comp> QueryChildren<Query, Answer, Comp>(Query query) where Comp : Component
		{
			return new QueryResults<Query, Answer, Comp>(Children, query);
		}

		public void SendMessageToChildren<Query>(Query query)
		{
			for (int i = 0; i < Children.Count; ++i)
			{
				Children[i].Host.SendMessage<Query>(query);
			}
		}

		public Answer QueryParent<Query, Answer, Comp>(Query query) where Comp : Component
		{
			if (Parent != null)
			{
				var comp = Parent.Host.GetComponent<Comp>();
				if (comp != null)
				{
					var func = comp as IMessageResponder<Query, Answer>;
					return func != null ? func.HandleMessage(query) : default(Answer);
				}
				else
				{
					return default(Answer);
				}
			}
			else return default(Answer);
		}

		public void SendMessageToParent<Query>(Query query)
		{
			if (Parent != null)
			{
				Parent.Host.SendMessage<Query>(query);
			}
		}

		/// <summary>
		/// Sends message downwards (parent -> child). Depth-first.
		/// </summary>
		/// <typeparam name="Message"></typeparam>
		/// <param name="msg"></param>
		public void PropagateMessageDownwards<Message>(Message msg)
		{
			Host.SendMessage(msg);
			foreach (var tree in Children) tree.PropagateMessageDownwards(msg);
		}
	}
}
