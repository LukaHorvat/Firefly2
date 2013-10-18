using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class ComponentCollectionChanged
	{
		public enum ChangeType
		{
			Add,
			Remove
		}

		public Component Target;
		public ChangeType Type;

		public ComponentCollectionChanged(Component target, ChangeType type)
		{
			Target = target;
			Type = type;
		}
	}
}
