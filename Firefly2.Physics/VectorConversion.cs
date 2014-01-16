using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Physics
{
	class VectorConversion
	{
		public static Microsoft.Xna.Framework.Vector2 Convert(OpenTK.Vector2d vec)
		{
			return new Microsoft.Xna.Framework.Vector2((float)vec.X, (float)vec.Y);
		}

		public static OpenTK.Vector2d Convert(Microsoft.Xna.Framework.Vector2 vec)
		{
			return new OpenTK.Vector2d(vec.X, vec.Y);
		}
	}
}
