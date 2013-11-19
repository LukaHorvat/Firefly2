using Firefly2;
using Firefly2.Components;
using Firefly2.Facilities;
using Firefly2.Messages.Querying;
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

			var circle = new Entity{
				new RenderBufferComponent(stage.Renderer),
				new TransformComponent(),
				new GeometryComponent(),
				new ShapeColorComponent(),
				new TreeNodeComponent()
			};

			for (int i = 0; i <= 361; ++i)
			{
				circle.GetComponent<GeometryComponent>().Add(new Vector2d(Math.Cos(i / 180F * Math.PI) * 200, Math.Sin(i / 180F * Math.PI) * 200));
				circle.GetComponent<ShapeColorComponent>().Add(Color.HSVToRGB(i, 1, 1, 1));
			}
			circle.GetComponent<GeometryComponent>().Add(new Vector2d(0, 0));
			circle.GetComponent<ShapeColorComponent>().Add(Color.HSVToRGB(0, 0, 0, 1));
			stage.TreeNode.AddChild(circle);

			stage.Run();
		}

		static Entity MakeRectangle(Renderer renderer, MutableVector2 mouse)
		{
			return new Entity
			{
				new GeometryComponent
				{
					new Vector2d(0, 0),
					new Vector2d(200, 0),
					new Vector2d(200, 100),
					new Vector2d(0, 100)
				},
				new ShapeColorComponent
				{
					new Vector4(1, 0, 0, 1),
					new Vector4(0, 1, 0, 1),
					new Vector4(0, 0, 1, 1),
					new Vector4(1, 1, 0, 1)
				},
				new RenderBufferComponent(renderer),
				new TreeNodeComponent(),
				new TransformComponent(),
				new MouseInteractionComponent(mouse)
			};
		}
	}
}
