using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildProject
{
	interface IHasTreeNode
	{
		TreeNodeComponent TreeNode { get; set; }
	}
}
