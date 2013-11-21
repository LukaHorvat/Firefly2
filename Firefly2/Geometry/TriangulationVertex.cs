using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet.Data;

namespace Firefly2.Geometry
{
	class TriangulationVertex : Vertex
	{
		public VertexData Data;
		public TriangulationVertex(VertexData data)
			: base(data.Coordinates.X, data.Coordinates.Y)
		{
			Data = data;
		}
	}
}
