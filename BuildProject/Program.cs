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

			var world = new Entity
			{
				new TransformComponent(),
				new TreeNodeComponent(),
				new CameraComponent()
			};
			stage.TreeNode.AddChild(world);

			var a = MakeRectangle(stage.Renderer, stage.GetMouse());
			var b = MakeRectangle(stage.Renderer, stage.GetMouse());
			var c = MakeRectangle(stage.Renderer, stage.GetMouse());
			b.GetComponent<TransformComponent>().X = 100;
			c.GetComponent<TransformComponent>().X = 200;

			stage.Update.Update += delegate
			{
				new List<Entity>() { a, b, c }.ForEach(ent =>
				{
					var intersects = ent.GetComponent<MouseInteractionComponent>().IntersectsMouse;
					if (intersects)
					{
						ent.GetComponent<TransformComponent>().ScaleX = 0.6;
						ent.GetComponent<TransformComponent>().ScaleY = 0.6;
					}
					else
					{
						ent.GetComponent<TransformComponent>().ScaleX = 0.5;
						ent.GetComponent<TransformComponent>().ScaleY = 0.5;
					}
				});
			};

			world.GetComponent<CameraComponent>().LookAt(100, 100);

			world.GetComponent<TreeNodeComponent>().AddChild(a);
			world.GetComponent<TreeNodeComponent>().AddChild(b);
			world.GetComponent<TreeNodeComponent>().AddChild(c);

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
