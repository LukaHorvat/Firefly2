using Firefly2;
using Firefly2.Components;
using Firefly2.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildProject
{
	class Program
	{
		static void Main(string[] args)
		{
			var stage = new Stage(800, 500, "Hello World");

			stage.TreeNode.AddChild(new Entity
			{
				new GeometryComponent
				{
					new Vector2d(0, 0),
					new Vector2d(100, 0),
					new Vector2d(100, 10),
					new Vector2d(0, 10)
				},
				new ShapeColorComponent
				{
					new Vector4(1, 0, 0, 1),
					new Vector4(0, 1, 0, 1),
					new Vector4(0, 0, 1, 1),
					new Vector4(1, 1, 0, 1)
				},
				new RenderBufferComponent(stage.Renderer),
				new TreeNodeComponent()
			});

			stage.Run();
		}
	}
}
