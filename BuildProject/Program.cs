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
			var rand = new Random();

			var stage = new Stage(800, 500, "Test");
			for (int i = 0; i < 8190; ++i)
			{
				var test = new Particle(stage);
				test.GetComponent<TransformComponent>().X = rand.Next(-400, 400);
				test.GetComponent<TransformComponent>().Y = rand.Next(-250, 250);
				stage.GetComponent<TreeNodeComponent>().Children.Add(test.GetComponent<TreeNodeComponent>());
			}

			stage.Run();

			Console.ReadKey();
		}
	}
}
