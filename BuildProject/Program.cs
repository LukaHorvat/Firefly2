using Firefly2;
using Firefly2.Components;
using Firefly2.Facilities;
using Firefly2.Messages;
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

			var rec1 = new Rectangle(100, 100, Color.HSVToRGB(0, 0.7, 1, 1));
			var rec2 = new Rectangle(100, 100, Color.HSVToRGB(90, 0.7, 1, 1));
			stage.TreeNode.AddChild(rec1);
			stage.TreeNode.AddChild(rec2);
			rec1.Transform.Z = -0.5;
			rec2.Transform.Z = -1;

			double acc = 0;
			stage.Update.AfterUpdate += delegate(AfterUpdateMessage msg)
			{
				acc += msg.DeltaTime;
				rec1.Transform.X = Math.Cos(acc) * 200;
				rec1.Transform.Z = Math.Sin(acc);
				rec1.Transform.ScaleX = rec1.Transform.ScaleY = 1 / (rec1.Transform.Z + 2);

				rec2.Transform.X = Math.Cos(acc + Math.PI) * 200;
				rec2.Transform.Z = Math.Sin(acc + Math.PI);
				rec2.Transform.ScaleX = rec2.Transform.ScaleY = 1 / (rec2.Transform.Z + 2);
			};

			stage.Run();
		}
	}
}
