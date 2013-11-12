using Firefly2.Components;
using Firefly2.Facilities;
using Firefly2.Messages;
using Firefly2.Utility;
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

		[Shorthand]
		public TreeNodeComponent TreeNode { get; set; }

		[Shorthand]
		public UpdateComponent Update { get; set; }

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

			AddComponent<TreeNodeComponent>();
			AddComponent<UpdateComponent>();

			Renderer = new Renderer(width, height);
		}

		public void Run()
		{
			Window.Run();
		}

		private MutableVector2 mouse;
		public MutableVector2 GetMouse()
		{
			if (mouse != null) return mouse;
			var m = Window.Mouse;
			var vec = new MutableVector2(0, 0);
			m.Move += delegate
			{
				vec.X = m.X - Window.Width / 2;
				vec.Y = m.Y - Window.Height / 2;
			};
			mouse = vec;
			return vec;
		}
	}
}
