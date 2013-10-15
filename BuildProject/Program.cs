using Firefly2;
using Firefly2.Components;
using Firefly2.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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
			var rand = new Random();

			var stage = new Stage(800, 500, "Test");
			var stick1 = new Stick(stage, "a");
			var stick2 = new Stick(stage, "b");
			stick2.GetComponent<TransformComponent>().X = 100;
			stage.GetComponent<TreeNodeComponent>().AddChild(stick1);
			stick1.GetComponent<TreeNodeComponent>().AddChild(stick2);
			stick1.GetComponent<TransformComponent>().X = 1;

			stage.GetComponent<UpdateComponent>().AfterUpdate += delegate
			{
				stick1.Transform.Rotation += 0.01;
				//stick1.GetComponent<TransformComponent>().Rotation += 0.01;
				//stick2.GetComponent<TransformComponent>().Rotation += 0.01;
				//Console.WriteLine(stick2.GetComponent<TransformComponent>().X);
			};

			stage.Run();

			Console.ReadKey();
		}
	}
}
