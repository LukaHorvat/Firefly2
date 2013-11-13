using Firefly2.Components;
using Firefly2.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class CameraComponent : Component
	{
		private Dictionary<Stage, MutableVector2> mice;

		public CameraComponent()
		{
			mice = new Dictionary<Stage, MutableVector2>();
		}

		public void LookAt(double x, double y)
		{
			if (Host == null) return;
			var transform = Host.GetComponent<TransformComponent>();
			if (transform == null) return;
			var transl = transform.TransformPoint(new Vector4((float)x, (float)y, 0, 1));
			transform.X = -transl.X;
			transform.Y = -transl.Y;
		}
	}
}
