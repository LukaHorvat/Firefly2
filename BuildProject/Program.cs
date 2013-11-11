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
			var a = new Node();
			var b = new Node();
			var c = new Node();

			a.Tree.AddChild(b);
			b.Tree.AddChild(c);

			var link = stage.TreeNode.CreateDownlink<UpdateComponent>();
			c.AddComponent<UpdateComponent>();
			stage.TreeNode.AddChild(a);
			a.Tree.RemoveChild(b);

			link = null;
			GC.Collect();

			link = stage.TreeNode.CreateDownlink<UpdateComponent>();

			stage.Run();

			Console.ReadKey();
		}
	}
}
