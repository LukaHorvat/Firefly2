using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2
{
	public abstract class Component
	{
		public Entity Host;
		public readonly string Name;

		public Component()
		{
			Name = this.GetType().Name;
		}
	}
}
