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
			//var bmp = new Bitmap(360, 10);
			//for (int i = 0; i < 360; ++i)
			//{
			//	for (int j = 0; j < 10; ++j)
			//	{
			//		var col = Firefly2.Utility.Color.HSVToRGB(i, 1, 1, 1);
			//		bmp.SetPixel(i, j, System.Drawing.Color.FromArgb((int)(col.X * 255), (int)(col.Y * 255), (int)(col.Z * 255)));
			//	}
			//}
			//bmp.Save("C:/pic.png");

			var stage = new Stage(800, 500, "Hello World");

			var rectangle = new Entity{
				new RenderBufferComponent(stage.Renderer),
				new TransformComponent(),
				new GeometryComponent(),
				new ShapeColorComponent(),
				new TreeNodeComponent()
			};
			//for (int i = 0; i < 360; ++i)
			//{
			//	rectangle.GetComponent<GeometryComponent>().Add(new Vector2d(i, 0));
			//	rectangle.GetComponent<ShapeColorComponent>().Add(Color.HSVToRGB(i, 1, 1, 1));
			//}
			//for (int i = 360; i < 0; ++i)
			//{
			//	rectangle.GetComponent<GeometryComponent>().Add(new Vector2d(i, 100));
			//	rectangle.GetComponent<ShapeColorComponent>().Add(Color.HSVToRGB(i, 1, 1, 1));
			//}

			for (int i = 0; i < 360; ++i)
			{
				rectangle.GetComponent<GeometryComponent>().Add(new Vector2d(Math.Cos(i / 180F * Math.PI) * 200, Math.Sin(i / 180F * Math.PI) * 200));
				rectangle.GetComponent<ShapeColorComponent>().Add(Color.HSVToRGB(i, 1, 1, 1));
			}
			rectangle.GetComponent<GeometryComponent>().Add(new Vector2d(0, 0));
			rectangle.GetComponent<ShapeColorComponent>().Add(Color.HSVToRGB(0, 0, 0, 1));
			stage.TreeNode.AddChild(rectangle);

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
