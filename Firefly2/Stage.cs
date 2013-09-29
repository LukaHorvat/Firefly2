using Firefly2.Components;
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
		public GameWindow Window;

		protected TreeNodeComponent TreeNode
		{
			get
			{
				return GetComponent<TreeNodeComponent>();
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
				TreeNode.PropagateMessageDownwards(new UpdateMessage(args.Time));
				TreeNode.PropagateMessageDownwards(new AfterUpdateMessage(args.Time));
			};

			Window.RenderFrame += delegate
			{
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);

				//RENDER

				Window.SwapBuffers();
			};

			Components.Add(new TreeNodeComponent());

			Window.Run();
		}
	}
}
