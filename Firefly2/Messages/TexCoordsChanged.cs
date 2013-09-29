using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class TexCoordsChanged
	{
		public static TexCoordsChanged Instance = new TexCoordsChanged();

		private TexCoordsChanged() { }
	}
}
