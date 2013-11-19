using Firefly2.Templating.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Templating
{
	public class EntityGenerator
	{
		public static string GenerateClasses()
		{
			string file = @"
using Firefly2;
using Firefly2.Components;";


			var asm = Assembly.GetCallingAssembly();

			//Process fields
			foreach (var type in asm.GetTypes())
			{
				foreach (var member in type.GetMembers())
				{
					if (EntityClass.IsDefined(member, type))
					{
						EntityClass attribute = (EntityClass)EntityClass.GetCustomAttribute(member, type, false);
						file += GenerateClass(attribute.ClassName, attribute.Components);
					}
				}
			}

			return file;
		}

		private static string GenerateClass(string name, IncludeComponent[] components)
		{
			//Generate shorthands
			string shorthands = "";
			foreach (var component in components)
			{
				var componentName = component.ComponentType.Name;
				if (componentName.Substring(componentName.Length - 9) == "Component")
				{
					componentName = componentName.Substring(0, componentName.Length - 9);
				}
				shorthands += string.Format(@"
	[Shorthand]
	public {0} {1} { get; set; }",
				componentName, component.ComponentType.Name);
			}

			//Generate add calls
			string addCalls = "";
			foreach (var component in components)
			{
				addCalls += string.Format(@"
		AddComponent<{0}>();
",
				component.ComponentType.Name);
			}

			return String.Format(@"
public class {0} : Entity
{
{1}

	public {0}()
	{
{2}
	}
}",
			name, shorthands, addCalls);
		}
	}
}
