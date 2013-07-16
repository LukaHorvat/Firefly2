using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class TreeNode : Component
	{
		public ObservableCollection<TreeNode> Children;
		public TreeNode Parent;

		public TreeNode() : this(null) { }

		public TreeNode(TreeNode parent)
		{
			Parent = parent;
			Children = new ObservableCollection<TreeNode>();
		}

		public void PropagateDownwards(object message, Action<object> response)
		{

		}
	}
}
