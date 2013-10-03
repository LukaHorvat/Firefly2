using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	public class PriorityQueue<T> : IEnumerable<T>
	{
		Func<T, T, int> comparer;
		BinaryTree<T> tree;

		public int Count { get { return tree.Count; } }

		public PriorityQueue(Func<T, T, int> comparer)
		{
			this.comparer = comparer;
			tree = new BinaryTree<T>(comparer);
		}

		public void Enqueue(T value)
		{
			tree.Add(value);
		}

		public T Dequeue()
		{
			var r = tree.Min;
			tree.Remove(r);
			return r;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return tree.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return tree.GetEnumerator();
		}
	}
}
