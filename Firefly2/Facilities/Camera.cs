using Firefly2.Components;
using Firefly2.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class Camera
	{
		private Dictionary<Stage, MutableVector2> mice;
		public Entity World;

		public Camera(Entity world)
		{
			mice = new Dictionary<Stage, MutableVector2>();
			World = world;
		}

		public void LookAt(double x, double y)
		{
			var transl = World.GetComponent<TransformComponent>().TransformPoint(new Vector4((float)x, (float)y, 0, 1));
			World.GetComponent<TransformComponent>().X = -transl.X;
			World.GetComponent<TransformComponent>().Y = -transl.Y;
		}
	}
}
