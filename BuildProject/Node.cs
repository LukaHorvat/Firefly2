using Firefly2;
using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildProject
{
	class Node : Entity
	{
		[Shorthand]
		public TreeNodeComponent Tree { get; set; }

		public Node()
		{
			Components.Add(new TreeNodeComponent());
		}
	}
}
