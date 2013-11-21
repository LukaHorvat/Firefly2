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
				(geom.Points as List<Vertex>).Add(new TriangulationVertex(point));
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
