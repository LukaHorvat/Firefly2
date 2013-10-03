using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	class Geometry
	{
		public static double CCW(Vector2d a, Vector2d b, Vector2d c)
		{
			return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
		}
	}
}
