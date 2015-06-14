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

			var picture = new Rectangle(100, 100, Color.HSVToRGB(90, 0, 0, 0));
			picture.AddComponent<TextureComponent>();
			picture.GetComponent<TextureComponent>().TexCoords.AddMany(
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			);
			picture.GetComponent<TextureComponent>().UpdateBitmap(new System.Drawing.Bitmap("EdibleAnus.png"));

			var fish = new Rectangle(100, 100, Color.HSVToRGB(180, 1, 1, 1));
			//fish.AddComponent<TextureComponent>();
			//fish.GetComponent<TextureComponent>().TexCoords.AddMany(
			//	new Vector2(0, 0),
			//	new Vector2(1, 0),
			//	new Vector2(1, 1),
			//	new Vector2(0, 1)
			//);
			//fish.GetComponent<TextureComponent>().UpdateBitmap(new System.Drawing.Bitmap("BetaFish.jpg"));
			fish.Transform.X = 100;

			world.GetComponent<TreeNodeComponent>().AddChild(picture);
			world.GetComponent<TreeNodeComponent>().AddChild(fish);

			stage.Run();
		}
	}
}
