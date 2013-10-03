using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class ParentTransformChanged
	{
		public Matrix4 Matrix;

		public ParentTransformChanged(Matrix4 mat)
		{
			Matrix = mat;
		}
	}
}
