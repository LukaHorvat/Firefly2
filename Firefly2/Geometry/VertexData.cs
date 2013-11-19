using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Geometry
{
	internal class VertexData
	{
		public Vector2d Coordinates;
		public Vector4 Color;
		public Vector2 TexCoords;
		public short TransformIndex;

		public VertexData(Vector2d coords, Vector4 color, Vector2 texCoords, short transformIndex)
		{
			Coordinates = coords;
			Color = color;
			TexCoords = texCoords;
			TransformIndex = transformIndex;
		}

		public VertexData(Vector2d coords, short transformIndex)
		{
			Coordinates = coords;
			TransformIndex = transformIndex;
		}

		public void WriteToArray(byte[] arr, int index)
		{
			using (var writer = new BinaryWriter(new MemoryStream(arr, index, 16)))
			{
				writer.Write((float)Coordinates.X);
				writer.Write((float)Coordinates.Y);
				writer.Write((byte)(Color.X * 255));
				writer.Write((byte)(Color.Y * 255));
				writer.Write((byte)(Color.Z * 255));
				writer.Write((byte)(Color.W * 255));
				writer.Write((byte)(TexCoords.X * 255));
				writer.Write((byte)(TexCoords.Y * 255));
				writer.Write(TransformIndex);

				writer.BaseStream.Dispose();
			}
		}
	}
}
