using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class ShaderProgramInfo
	{
		public int ShaderProgram;
		public int VertexPosition, VertexColor, VertexTexCoords, VertexIndex;
		public int TexBuffer;
		public int WindowLocation;

		public Matrix4 Window;
	}
}
