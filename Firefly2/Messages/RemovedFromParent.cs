using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class RemovedFromParent
	{
		public static RemovedFromParent Instance = new RemovedFromParent();

		private RemovedFromParent() { }
	}
}
