using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	public class Color
	{
		public static Vector4 HSVToRGB(float hue, float saturation, float value, float alpha)
		{
			Func<float, float> helper = (x) =>
			{
				float lowerBound = value * (1 - saturation);
				float delta = value - lowerBound;
				while (x < 0) x += 360;
				x = x % 360;
				return lowerBound + (float)((Math.Floor(x / 180) * value) + Math.Floor(((Math.Floor(x / 60) % 3) / 2)) * (x % 60) * (delta / 60F) * Math.Pow(-1, Math.Floor(x / 180)));
			};

			return new Vector4(helper(hue - 120), helper(360 - hue), helper(hue), alpha);
		}
	}
}
