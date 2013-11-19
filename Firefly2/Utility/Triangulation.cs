using Firefly2.Geometry;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;

namespace Firefly2.Utility
{
	internal class Triangulation
	{
		public static List<Triangle<VertexData>> Triangulate(IList<VertexData> points)
		{
			var mesh = new Mesh();
			var geom = new InputGeometry();
			points.ForEach((point, index) =>
			{
				//We use the boundary parameter as an index so we can later attach VertexData to
				//each vertex
				geom.AddPoint(point.Coordinates.X, point.Coordinates.Y, index);
				geom.AddSegment(index, (index + 1) % points.Count);
			});
			mesh.Triangulate(geom);
			return mesh.Triangles.Select(triangle =>
			{
				return new Triangle<VertexData>(
					points[triangle.GetVertex(0).Boundary],
					points[triangle.GetVertex(1).Boundary],
					points[triangle.GetVertex(2).Boundary]);
			})
			.ToList();
		}
	}
}
