using Firefly2;
using Firefly2.Algorithm;
using Firefly2.Components;
using Firefly2.Facilities;
using Firefly2.Messages;
using Firefly2.Messages.Querying;
using Firefly2.Utility;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
			var world = new Entity();
			world.AddComponent<TreeNodeComponent>();
			world.AddComponent<TransformComponent>();
			world.AddComponent<CameraComponent>();
			stage.TreeNode.AddChild(world);

			world.AddComponent<CameraComponent>();
			world.GetComponent<CameraComponent>().LookAt(400, 250);
			int number = 0;

			List<PackedRectangle> packed = null, packed2 = null;
			var rand = new Random(1000);
			var rects = new List<PackedRectangle>();
			number = 200;
			for (int i = 0; i < number; ++i)
			{
				rects.Add
				(
					new PackedRectangle
					(
						new Firefly2.Geometry.AARectangle
						(
							new Vector2d(0, 0),
							new Vector2d(rand.Next(20) + 60, rand.Next(20) + 60)
						),
						i
					)
				);
			}
			packed = RectanglePacking.PackRectangles(rects);
			packed2 = RectanglePacking.PackRectanglesFSharp(rects);
			packed = packed2;

			//for (int i = 0; i < number; ++i)
			//{
			//	if (packed[i].Rectangle.Position != packed2[i].Rectangle.Position)
			//	{
			//		Debugger.Break();
			//	}
			//}

			int count = 0;
			bool spaceDown = false;
			stage.Update.Update += delegate
			{
				var keyboard = OpenTK.Input.Keyboard.GetState();
				if (keyboard.IsKeyDown(Key.Space))
				{
					spaceDown = true;
				}
				if (keyboard.IsKeyUp(Key.Space))
				{
					if (spaceDown)
					{
						if (packed.Count == count) return;
						var rect = packed[count];
						var displayRect = new Rectangle
						(
							(float)rect.Rectangle.Size.X,
							(float)rect.Rectangle.Size.Y,
							Color.HSVToRGB(0, 0.5, 0.7F / number * count + 0.3F, 1)
						);
						displayRect.Transform.X = rect.Rectangle.Position.X;
						displayRect.Transform.Y = rect.Rectangle.Position.Y;
						world.GetComponent<TreeNodeComponent>().AddChild(displayRect);
						count++;
						spaceDown = false;
					}
				}
			};

			stage.Run();
		}
	}
}
