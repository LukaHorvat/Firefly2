using FarseerPhysics.Dynamics;
using Firefly2.Components;
using Firefly2.Messages;
using Firefly2.Utility;
using Microsoft.Xna.Framework;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Physics
{
	public class PhysicalSettings : Entity
	{
		public static PhysicalSettings Default = new PhysicalSettings();

		private ImmutableVector2 gravity;

		public float UnitsPerMeter = 100;
		public double MetersPerUnit
		{
			get { return 1 / UnitsPerMeter; }
			set { UnitsPerMeter = 1 / (float)value; }
		}
		public ImmutableVector2 Gravity
		{
			get { return gravity; }
			set
			{
				gravity = value;
				World.Gravity = new Microsoft.Xna.Framework.Vector2((float)gravity.X, (float)gravity.Y);
			}
		}
		public World World;

		public PhysicalSettings()
		{
			AddComponent<TreeNodeComponent>();
			AddComponent<UpdateComponent>();

			World = new World(new Microsoft.Xna.Framework.Vector2(0, 0));
			Gravity = new ImmutableVector2(0, 0);

			GetComponent<UpdateComponent>().Update += delegate(UpdateMessage msg)
			{
				World.Step((float)msg.DeltaTime);
			};
		}
	}
}
