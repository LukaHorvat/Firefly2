using Firefly2.Components;
using Firefly2.Facilities;
using Firefly2.Messages;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2
{
	public class Stage : Entity
	{
		public Renderer Renderer;
		public GameWindow Window;
		public Action<Vector2d> temp;

		protected TreeNodeComponent TreeNode
		{
			get
			{
				return GetComponent<TreeNodeComponent>();
			}
		}

		protected UpdateComponent Update
		{
			get
			{
				return GetComponent<UpdateComponent>();
			}
		}

		public Stage(int width, int height, string title)
		{
			Window = new GameWindow(
				width, height,
				new GraphicsMode(new ColorFormat(8, 8, 8, 8), 8, 8, 8),
				title,
				GameWindowFlags.Default, DisplayDevice.Default,
				3, 0, GraphicsContextFlags.ForwardCompatible);

			Window.Load += delegate
			{
				GL.ClearColor(Color4.Black);
			};

			Window.UpdateFrame += delegate(object target, FrameEventArgs args)
			{
				var updateMessage = new UpdateMessage(args.Time);
				var afterUpdateMessage = new AfterUpdateMessage(args.Time);
				Window.Title = Window.UpdateFrequency + ", " + Window.RenderFrequency;

				if (temp != null) temp(new Vector2d(Window.Mouse.X - Window.Width / 2, Window.Mouse.Y - Window.Height / 2));
				TreeNode.Send(updateMessage, TreeNodeComponent.SendRange.WholeTree);
				Update.TakeMessage(updateMessage);
				TreeNode.Send(afterUpdateMessage, TreeNodeComponent.SendRange.WholeTree);
				Update.TakeMessage(afterUpdateMessage);
				Renderer.FinishUpdate();
			};

			Window.RenderFrame += delegate
			{
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);

				//RENDER
				Renderer.Render();

				Window.SwapBuffers();
			};

			Components.Add(new TreeNodeComponent());
			Components.Add(new UpdateComponent());

			Renderer = new Renderer(width, height);
		}

		public void Run()
		{
			Window.Run();
		}
	}
}
