using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	public enum BinaryTreeNodeType
	{
		Lesser, Greater, Root
	}

	public class BinaryTreeNode<T>
	{
		private BinaryTreeNodeType type;
		private Func<T, T, int> comparer;
		private T val;
		private BinaryTreeNode<T> lesser, greater;

		public BinaryTree<T> Tree;
		public BinaryTreeNode<T> Parent;
		public bool Active = false;
		public T Value
		{
			get
			{
				return val;
			}
			set
			{
				val = value;
				Active = true;
			}
		}
		public BinaryTreeNode<T> Lesser
		{
			get { return lesser == null ? lesser = new BinaryTreeNode<T>(comparer, BinaryTreeNodeType.Lesser, this) { Tree = this.Tree } : lesser; }
		}
		public BinaryTreeNode<T> Greater
		{
			get { return greater == null ? greater = new BinaryTreeNode<T>(comparer, BinaryTreeNodeType.Greater, this) { Tree = this.Tree } : greater; }
		}

		public BinaryTreeNode(T value, Func<T, T, int> comparer, BinaryTreeNodeType type, BinaryTreeNode<T> parent)
		{
			Value = value;
			this.comparer = comparer;
			this.type = type;
			this.Parent = parent;
		}

		public BinaryTreeNode(Func<T, T, int> comparer, BinaryTreeNodeType type, BinaryTreeNode<T> parent)
		{
			this.comparer = comparer;
			this.type = type;
			this.Parent = parent;
		}

		public void Add(T value)
		{
			if (!Active)
			{
				Value = value;
			}
			else
			{
				int cmp = comparer(Value, value);
				if (cmp >= 0) Lesser.Add(value);
				else Greater.Add(value);
			}
		}

		public void Remove(T value)
		{
			if (!Active) throw new Exception("Trying to remove an element that doesn't exsist from the binary tree");
			if (comparer(Value, value) == 0)
			{
				if (!Lesser.Active && !Greater.Active) DisconnectFromParent();
				else if (!Lesser.Active)
				{
					if (type == BinaryTreeNodeType.Lesser)
					{
						Parent.lesser = Greater;
						Greater.type = BinaryTreeNodeType.Lesser;
					}
					else if (type == BinaryTreeNodeType.Greater) Parent.greater = Greater;
					else
					{
						Tree.Root = Greater;
						Greater.type = BinaryTreeNodeType.Root;
					}
					Greater.Parent = Parent;
				}
				else if (!Greater.Active)
				{
					if (type == BinaryTreeNodeType.Lesser) Parent.lesser = Lesser;
					else if (type == BinaryTreeNodeType.Greater)
					{
						Parent.greater = Lesser;
						Lesser.type = BinaryTreeNodeType.Greater;
					}
					else
					{
						Tree.Root = Lesser;
						Lesser.type = BinaryTreeNodeType.Root;
					}
					Lesser.Parent = Parent;
				}
				else
				{
					var current = Lesser;
					while (current.Active)
					{
						current = current.Greater;
					}
					Value = current.Parent.Value;
					current.Parent.Remove(current.Parent.Value);
				}
			}
			else
			{
				int cmp = comparer(Value, value);
				if (cmp >= 0) Lesser.Remove(value);
				else Greater.Remove(value);
			}
		}

		public void DisconnectFromParent()
		{
			if (type == BinaryTreeNodeType.Lesser)
			{
				Parent.lesser = null;
			}
			else if (type == BinaryTreeNodeType.Greater)
			{
				Parent.greater = null;
			}
			Parent = null;
		}

		public bool Contains(T value)
		{
			if (!Active) return false;
			var cmp = comparer(Value, value);
			if (cmp == 0) return true;
			else if (cmp > 0) return Lesser.Contains(value);
			else return Greater.Contains(value);
		}

		public override string ToString()
		{
			return Active ? "Active: " + Value : "Inactive";
		}
	}

	public class BinaryTree<T> : IEnumerable<T>
	{
		public BinaryTreeNode<T> Root;
		Func<T, T, int> comparer;
		public int Count { get; private set; }

		private T min, max;
		public T Min
		{
			get
			{
				if (Count == 0) throw new Exception("Empty collection has no minimum");
				return min;
			}
			private set
			{
				min = value;
			}
		}
		public T Max
		{
			get
			{
				if (Count == 0) throw new Exception("Empty collection has no maximum");
				return max;
			}
			private set
			{
				max = value;
			}
		}

		public BinaryTree(Func<T, T, int> comparer)
		{
			this.comparer = comparer;
			Root = new BinaryTreeNode<T>(comparer, BinaryTreeNodeType.Root, null) { Tree = this };
		}

		public void Add(T value)
		{
			Root.Add(value);
			Count++;
			if (Count == 1) min = max = value;
			else if (comparer(value, Min) < 0) Min = value;
			else if (comparer(value, Max) > 0) Max = value;
		}

		public void Remove(T value)
		{
			if (!Root.Active) throw new Exception("Trying to remove an element that doesn't exsist from the binary tree");
			Root.Remove(value);
			Count--;

			if (Count > 0)
			{
				if (comparer(value, Max) == 0)
				{
					BinaryTreeNode<T> current, lastActive = null;
					for (current = Root; current.Active; current = current.Greater) lastActive = current;
					Max = lastActive.Value;
				}
				if (comparer(value, Min) == 0)
				{
					BinaryTreeNode<T> current, lastActive = null;
					for (current = Root; current.Active; current = current.Lesser) lastActive = current;
					Min = lastActive.Value;
				}
			}
		}

		public bool Contains(T value)
		{
			return Root.Contains(value);
		}

		public IEnumerator<T> GetEnumerator()
		{
			var queue = new Queue<T>();
			Iterate(Root, (n) =>
			{
				queue.Enqueue(n);
			});
			while (queue.Count > 0) yield return queue.Dequeue();
		}

		private void Iterate(BinaryTreeNode<T> node, Action<T> callback)
		{
			if (!node.Active) return;
			Iterate(node.Lesser, callback);
			callback(node.Value);
			Iterate(node.Greater, callback);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			var queue = new Queue<T>();
			Iterate(Root, (n) =>
			{
				queue.Enqueue(n);
			});
			while (queue.Count > 0) yield return queue.Dequeue();
		}
	}
}
