using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Templating.Attributes
{
	public abstract class IncludeComponent : Attribute
	{
		public Type ComponentType;
		public IncludeComponent(Type componentType)
		{
			ComponentType = componentType;
		}
	}
}
