using Firefly2.Facilities;
using Firefly2.Geometry;
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
													ITakesMessage<TransformationChanged>,
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

		private Uplink<RenderBufferComponent> uplink;
		private bool needsUpdate = false;
		private RenderingStatus rendering = RenderingStatus.NotRenderingParentInvisible;
		private TreeNodeComponent tree
		{
			get { return Host.GetComponent<TreeNodeComponent>(); }
		}
		private TransformComponent transform
		{
			get { return Host.GetComponent<TransformComponent>(); }
		}
		private short transformIndex = -1;

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

			if (transformIndex == -1)
			{
				var transform = Host.GetComponent<TransformComponent>();
				Matrix4 mat = Matrix4.Identity;
				if (transform != null) mat = transform.ModelMatrix;
				transformIndex = Renderer.ProcessTransform(this, mat);
			}

			var poly = Triangulation.MakePolygon(Geometry.Polygon);
			for (int i = 0; i < poly.Points.Count; ++i)
			{
				(poly.Points[i] as TriangulationPoint).Data = new VertexData(Geometry.Polygon[i], transformIndex);
			}
			if (ShapeColor != null && ShapeColor.Colors.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < ShapeColor.Colors.Count; ++i)
				{
					(poly.Points[i] as TriangulationPoint).Data.Color = ShapeColor.Colors[i];
				}
			}
			if (Texture != null && Texture.TexCoords.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < Texture.TexCoords.Count; ++i)
				{
					(poly.Points[i] as TriangulationPoint).Data.TexCoords = Texture.TexCoords[i];
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
					var point = triangles[i].Points[j] as TriangulationPoint;
					VertexData pack;
					if (point == null) pack = new VertexData(new Vector2d(), new Vector4(), new Vector2(), transformIndex);
					else pack = point.Data;
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
			Renderer.RemoveTransform(this);
			transformIndex = -1;
			tree.Send(StopRendering.Instance, TreeNodeComponent.SendRange.ImmediateChildrenOnly);
		}

		public void TakeMessage(TransformationChanged msg)
		{
			var newIndex = Renderer.ProcessTransform(this, Host.GetComponent<TransformComponent>().ModelMatrix);
			if (transformIndex != newIndex)
			{
				transformIndex = newIndex;
				needsUpdate = true;
			}
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
