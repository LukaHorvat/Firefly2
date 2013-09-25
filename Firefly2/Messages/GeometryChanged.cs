using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class GeometryChanged
	{
		private GeometryChanged() { }

		public static GeometryChanged Instance = new GeometryChanged();
	}
}
