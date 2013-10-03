using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class NewChild
	{
		public TreeNodeComponent Child;

		public NewChild(TreeNodeComponent child)
		{
			Child = child;
		}
	}
}
