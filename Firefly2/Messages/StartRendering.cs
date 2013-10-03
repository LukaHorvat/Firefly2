using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class StartRendering
	{
		public static StartRendering Instance = new StartRendering();

		private StartRendering() { }
	}
}
