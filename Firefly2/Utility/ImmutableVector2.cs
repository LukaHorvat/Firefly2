using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	public class ImmutableVector2
	{
		public readonly double X, Y;
		public ImmutableVector2(double x, double y)
		{
			X = x;
			Y = y;
		}
	}
}
