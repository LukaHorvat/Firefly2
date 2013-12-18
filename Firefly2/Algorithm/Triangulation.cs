using Firefly2.Geometry;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace Firefly2.Algorithm
{
	internal static class Triangulation
	{
		public static List<Triangle<VertexData>> Triangulate(IList<VertexData> points)
		{
			var mesh = new Mesh();
			var geom = new InputGeometry();
			points.ForEach((point, index) =>
			{
				//Bypass the default add method so we can insert our own vertices
				(geom.Points as List<Vertex>).Add(new TriangulationVertex(point));
				geom.Bounds.Update(point.Coordinates.X, point.Coordinates.Y);
				geom.AddSegment(index, (index + 1) % points.Count);
			});
			mesh.Triangulate(geom);
			return mesh.Triangles.Select(triangle =>
			{
				return new Triangle<VertexData>(
					(triangle.GetVertex(0) as TriangulationVertex).Data,
					(triangle.GetVertex(1) as TriangulationVertex).Data,
					(triangle.GetVertex(2) as TriangulationVertex).Data);
			})
			.ToList();
		}
	}
}
