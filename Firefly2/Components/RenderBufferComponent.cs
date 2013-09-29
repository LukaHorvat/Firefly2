using Firefly2.Facilities;
using Firefly2.Messages;
using Firefly2.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class RenderBufferComponent : Component, IMessageTaker<GeometryChanged>,
													IMessageTaker<ShapeColorChanged>,
													IMessageTaker<TexCoordsChanged>,
													IMessageTaker<AfterUpdateMessage>
	{
		private class VertexData
		{
			public Vector2d Coordinates;
			public Vector4 Color;
			public Vector2 TexCoords;

			public VertexData(Vector2d coords, Vector4 color, Vector2 texCoords)
			{
				Coordinates = coords;
				Color = color;
				TexCoords = texCoords;
			}

			public VertexData(Vector2d coords)
			{
				Coordinates = coords;
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
				}
			}
		}

		private bool needsUpdate = false;
		private Dictionary<uint, VertexData> triangulationMap;

		public Renderer Renderer;
		public byte[] Data;

		protected ShapeColorComponent ShapeColor
		{
			get { return Host.GetComponent<ShapeColorComponent>(); }
		}

		protected TextureComponent Texture
		{
			get { return Host.GetComponent<TextureComponent>(); }
		}

		protected GeometryComponent Geometry
		{
			get { return Host.GetComponent<GeometryComponent>(); }
		}

		public RenderBufferComponent(Renderer renderer)
		{
			Renderer = renderer;
			triangulationMap = new Dictionary<uint, VertexData>();
		}

		public void TakeMessage(GeometryChanged msg)
		{
			needsUpdate = true;
		}

		public void TakeMessage(ShapeColorChanged msg)
		{
			needsUpdate = true;
		}

		public void TakeMessage(TexCoordsChanged msg)
		{
			needsUpdate = true;
		}

		public void TakeMessage(AfterUpdateMessage msg)
		{
			if (!needsUpdate) return;

			var poly = Triangulation.MakePolygon(Geometry.Polygon);
			for (int i = 0; i < poly.Points.Count; ++i)
			{
				triangulationMap[poly.Points[i].VertexCode] = new VertexData(Geometry.Polygon[i]);
			}
			if (ShapeColor.Colors.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < ShapeColor.Colors.Count; ++i)
				{
					triangulationMap[poly.Points[i].VertexCode].Color = ShapeColor.Colors[i];
				}
			}
			if (Texture.TexCoords.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < Texture.TexCoords.Count; ++i)
				{
					triangulationMap[poly.Points[i].VertexCode].TexCoords = Texture.TexCoords[i];
				}
			}
			var triangles = Triangulation.TriangulatePolygon(poly);
			if (Data.Length < triangles.Count * 3 * 16)
			{
				Data = new byte[triangles.Count * 3 * 16];
			}
			for (int i = 0; i < triangles.Count; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					var pack = triangulationMap[triangles[i].Points[j].VertexCode];
					pack.WriteToArray(Data, (i * 3 + j) * 16);
				}
			}
			Renderer.ProcessRenderBuffer(this);

			needsUpdate = false;
		}
	}
}
