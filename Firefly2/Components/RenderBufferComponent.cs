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
	public class RenderBufferComponent : Component, ITakesMessage<GeometryChanged>,
													ITakesMessage<ShapeColorChanged>,
													ITakesMessage<TexCoordsChanged>,
													ITakesMessage<NewTransformIndex>,
													ITakesMessage<AfterUpdateMessage>,
													ITakesMessage<StartRendering>,
													ITakesMessage<StopRendering>,
													ITakesMessage<AddedToEntity>,
													ITakesMessage<ComponentCollectionChanged>
	{
		enum RenderingStatus
		{
			Rendering,
			NotRenderingParentInvisible,
			NotRenderingSelfInvisible
		}

		private class VertexData
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

		private Uplink<RenderBufferComponent> uplink;
		private bool needsUpdate = false;
		private Dictionary<uint, VertexData> triangulationMap;
		private RenderingStatus rendering = RenderingStatus.NotRenderingParentInvisible;
		private TreeNodeComponent tree
		{
			get { return Host.GetComponent<TreeNodeComponent>(); }
		}
		private TransformComponent transform
		{
			get { return Host.GetComponent<TransformComponent>(); }
		}
		private short transformIndex = 0;

		public Renderer Renderer;
		public byte[] Data;

		private bool visible;
		public bool Visible{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				if (value && rendering == RenderingStatus.NotRenderingSelfInvisible) StartRender();
				else if (!value && rendering == RenderingStatus.Rendering)
				{
					StopRender();
					rendering = RenderingStatus.NotRenderingSelfInvisible;
				}
			}
		}

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
			Data = new byte[0];
		}

		public void TakeMessage(AddedToEntity msg)
		{
			if (transform != null) transformIndex = transform.ObjectIndex;

			needsUpdate = true;
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
			if (Geometry == null) return;

			var poly = Triangulation.MakePolygon(Geometry.Polygon);
			for (int i = 0; i < poly.Points.Count; ++i)
			{
				triangulationMap[poly.Points[i].VertexCode] = new VertexData(Geometry.Polygon[i], transformIndex);
			}
			if (ShapeColor != null && ShapeColor.Colors.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < ShapeColor.Colors.Count; ++i)
				{
					triangulationMap[poly.Points[i].VertexCode].Color = ShapeColor.Colors[i];
				}
			}
			if (Texture != null && Texture.TexCoords.Count == Geometry.Polygon.Count)
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

		public void TakeMessage(StartRendering msg)
		{
			if (rendering == RenderingStatus.NotRenderingParentInvisible) StartRender();
		}

		public void TakeMessage(StopRendering msg)
		{
			if (rendering == RenderingStatus.Rendering) StopRender();
		}

		private void StartRender()
		{
			rendering = RenderingStatus.Rendering;
			needsUpdate = true;
			tree.Send(StartRendering.Instance, TreeNodeComponent.SendRange.ImmediateChildrenOnly);
		}

		private void StopRender()
		{
			rendering = RenderingStatus.NotRenderingParentInvisible;
			Renderer.RemoveRenderBuffer(this);
			tree.Send(StopRendering.Instance, TreeNodeComponent.SendRange.ImmediateChildrenOnly);
		}

		public void TakeMessage(NewTransformIndex msg)
		{
			transformIndex = msg.Index;
			needsUpdate = true;
		}

		public void TakeMessage(ComponentCollectionChanged msg)
		{
			if (msg.Target is GeometryComponent || msg.Target is ShapeColorComponent || msg.Target is TextureComponent)
			{
				needsUpdate = true;
			}
			if (msg.Target is TreeNodeComponent)
			{
				if (Host is Stage)
				{
					Visible = true;
					rendering = RenderingStatus.Rendering;
				}
				var treeNode = msg.Target as TreeNodeComponent;
				uplink = tree.CreateUplink<RenderBufferComponent>();
				uplink.EndpointChanged += delegate
				{
					bool parentWantsToRender =
						(Host is Stage) ||
						(uplink.Component != null && uplink.Component.visible);
					if (rendering == RenderingStatus.NotRenderingParentInvisible && parentWantsToRender)
					{
						StartRender();
					}
					else if (rendering == RenderingStatus.Rendering && !parentWantsToRender)
					{
						StopRender();
					}
				};
			}
		}
	}
}
