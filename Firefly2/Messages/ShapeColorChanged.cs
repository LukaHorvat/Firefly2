using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class ShapeColorChanged
	{
		private ShapeColorChanged() { }

		public static ShapeColorChanged Instance = new ShapeColorChanged();
	}
}
