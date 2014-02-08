using Firefly2.Algorithm;
using Firefly2.Facilities;
using Firefly2.Geometry;
using Firefly2.Messages;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class RenderBufferComponent : Component, ITakesMessage<GeometryChanged>,
													ITakesMessage<ShapeColorChanged>,
													ITakesMessage<TexCoordsChanged>,
													ITakesMessage<TextureChanged>,
													ITakesMessage<TransformationChanged>,
													ITakesMessage<RendererChanged>,
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

		private Bitmap bmp;
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
		private short objectIndex = -1;

		private Renderer renderer;
		public Renderer Renderer
		{
			get { return renderer; }
			set
			{
				if (renderer == value) return;

				if (renderer != null)
				{
					renderer.RemoveRenderBuffer(this);
					renderer.RemoveTransform(this);
					renderer.RemoveTexture(this);
				}

				value.ProcessRenderBuffer(this);

				needsUpdate = true;
				renderer = value;
				ResetRenderer();

				if (tree != null) tree.Send(new RendererChanged(value), TreeNodeComponent.SendRange.ImmediateChildrenOnly);
			}
		}
		public byte[] Data;

		private bool visible;
		public bool Visible
		{
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

		public RenderBufferComponent()
			: this(null) { }

		public RenderBufferComponent(Renderer renderer)
		{
			this.renderer = renderer;
			Data = new byte[0];
		}

		public void TakeMessage(AddedToEntity msg)
		{
			//Make sure the index updates on the next update
			objectIndex = -1;

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
			if (renderer == null) return;
			if (!needsUpdate) return;
			if (Geometry == null) return;

			if (objectIndex == -1)
			{
				ResetRenderer();
			}

			var poly = Geometry.Polygon.Select(vec => new VertexData(vec, objectIndex)).ToList();
			if (ShapeColor != null && ShapeColor.Colors.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < ShapeColor.Colors.Count; ++i)
				{
					poly[i].Color = ShapeColor.Colors[i];
				}
			}
			if (Texture != null && Texture.TexCoords.Count == Geometry.Polygon.Count)
			{
				for (int i = 0; i < Texture.TexCoords.Count; ++i)
				{
					poly[i].TexCoords = Texture.TexCoords[i];
				}
			}
			var triangles = Triangulation.Triangulate(poly);
			if (Data.Length < triangles.Count * 3 * 16)
			{
				Data = new byte[triangles.Count * 3 * 16];
			}
			for (int i = 0; i < triangles.Count; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					var pack = triangles[i].Points[j];
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
			Renderer.RemoveTexture(this);
			objectIndex = -1;
			tree.Send(StopRendering.Instance, TreeNodeComponent.SendRange.ImmediateChildrenOnly);
		}

		public void TakeMessage(TransformationChanged msg)
		{
			if (Renderer == null) return;
			var newIndex = Renderer.ProcessTransform(this, Host.GetComponent<TransformComponent>().ModelMatrix);
			if (objectIndex != newIndex)
			{
				objectIndex = newIndex;
				needsUpdate = true;
			}
		}

		public void TakeMessage(RendererChanged msg)
		{
			if (msg.NewRenderer != Renderer) Renderer = msg.NewRenderer;
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
					if (uplink.Component == null && !(Host is Stage))
					{
						StopRender();
						return;
					}
					if (uplink.Component.Renderer != Renderer) Renderer = uplink.Component.Renderer;

					bool parentWantsToRender = Host is Stage || uplink.Component.visible;
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

		public void TakeMessage(TextureChanged msg)
		{
			bmp = msg.NewTexture;
			if (Renderer == null) return;
			var newIndex = Renderer.ProcessTexture(this, msg.NewTexture);
			if (objectIndex != newIndex)
			{
				objectIndex = newIndex;
				needsUpdate = true;
			}
		}

		private void ResetRenderer()
		{
			var transform = Host.GetComponent<TransformComponent>();
			Matrix4 mat = Matrix4.Identity;
			if (transform != null) mat = transform.ModelMatrix;
			objectIndex = Renderer.ProcessTransform(this, mat);
			if (bmp != null) Renderer.ProcessTexture(this, bmp);
		}
	}
}
