using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Templating.Attributes
{
	public class EntityClass : Attribute
	{
		public IncludeComponent[] Components;
		public string ClassName;

		public EntityClass(string className, params IncludeComponent[] components)
		{
			ClassName = className;
			Components = components;
		}		
	}
}
