using Firefly2.Geometry;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poly2Tri;

namespace Firefly2.Utility
{
	public class Triangulation
	{
		public static Polygon MakePolygon(IEnumerable<Vector2d> poly)
		{
			return new Polygon(poly.Select(vec => new PolygonPoint(vec.X, vec.Y)));
		}

		public static IList<DelaunayTriangle> TriangulatePolygon(Polygon poly)
		{
			P2T.Triangulate(poly);
			return poly.Triangles;
		}

		public static List<Triangle> TriangulatePolygon(IEnumerable<Vector2d> poly)
		{
			return TriangulatePolygon(MakePolygon(poly)).Select(Triangle.FromDelaunay).ToList();
		}
	}
}
