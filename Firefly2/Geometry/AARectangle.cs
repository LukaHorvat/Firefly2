using Firefly2.Functional.RectanglePacking;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Geometry
{
	public class AARectangle
	{
		public Vector2d Position;
		public Vector2d Size;

		public AARectangle(Vector2d position, Vector2d size)
		{
			Position = position;
			Size = size;
		}

		public AARectangle(double x, double y, double width, double height)
			: this(new Vector2d(x, y), new Vector2d(width, height)) { }
	}
}
