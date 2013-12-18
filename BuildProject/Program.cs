using Firefly2;
using Firefly2.Components;
using Firefly2.Facilities;
using Firefly2.Messages;
using Firefly2.Messages.Querying;
using Firefly2.Physics;
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
			stage.TreeNode.AddChild(PhysicalSettings.Default);

			var rec = new Rectangle(100, 100, new Vector4(1, 0, 0, 1));
			rec.Transform.Y = 100;
			rec.AddComponent<PhysicsComponent>();
			var rec2 = new Rectangle(100, 100, new Vector4(1, 1, 0, 1));
			rec2.Transform.Y = 0;
			rec2.AddComponent<PhysicsComponent>();

			rec.GetComponent<PhysicsComponent>().AddJoint(rec2, new Vector2d(0, -50), new Vector2d(0, 50));

			stage.TreeNode.AddChild(rec);
			stage.TreeNode.AddChild(rec2);

			stage.Update.AfterUpdate+= delegate
			{
				rec.GetComponent<PhysicsComponent>().ApplyForce(new Vector2d(1, 0), new Vector2d(0, 0));
			};

			stage.Run();
		}
	}
}
