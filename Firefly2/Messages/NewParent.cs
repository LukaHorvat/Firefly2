using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class NewParent
	{
		public static NewParent Instance = new NewParent();

		private NewParent() { }
	}
}
