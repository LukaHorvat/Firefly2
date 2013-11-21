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
		/// <summary>
		/// Convert a HSV color to RGB
		/// </summary>
		/// <param name="hue">In degrees</param>
		/// <param name="saturation">0-1</param>
		/// <param name="value">0-1</param>
		/// <param name="alpha">0-1</param>
		/// <returns></returns>
		public static Vector4 HSVToRGB(double hue, double saturation, double value, double alpha)
		{
			//Function written by looking at this diagram http://en.wikipedia.org/wiki/File:HSV-RGB-comparison.svg
			Func<double, float> helper = (x) =>
			{
				double lowerBound = value * (1 - saturation);
				double delta = value - lowerBound;
				while (x < 0) x += 360;
				x = x % 360;
				return (float)(lowerBound + 
					(float)((Math.Floor(x / 180) * delta) + 
					Math.Floor(((Math.Floor(x / 60) % 3) / 2)) * (x % 60) * (delta / 60F) * Math.Pow(-1, Math.Floor(x / 180))));
			};

			return new Vector4(helper(hue - 120), helper(360 - hue), helper(hue), (float)alpha);
		}
	}
}
