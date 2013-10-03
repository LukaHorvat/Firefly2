using OpenTK;
using Poly2Tri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Geometry
{
	public class Triangle
	{
		Vector2d[] arr;

		public Vector2d A { get { return arr[0]; } }
		public Vector2d B { get { return arr[1]; } }
		public Vector2d C { get { return arr[2]; } }

		public Triangle(Vector2d a, Vector2d b, Vector2d c)
		{
			arr = new[] { a, b, c };
		}

		public static Triangle FromDelaunay(DelaunayTriangle tri)
		{
			TriangulationPoint a = tri.Points[0], b = tri.Points[1], c = tri.Points[2];
			return new Triangle(new Vector2d(a.X, a.Y), new Vector2d(b.X, b.Y), new Vector2d(c.X, c.Y));
		}
	}
}
