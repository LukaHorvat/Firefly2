using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	public class Debug
	{
		static StringBuilder builder = new StringBuilder();

		public static void WriteListSeparated<T>(IEnumerable<T> list, string separator)
		{
			foreach (var item in list)
			{
				builder.Append(item + separator);
			}
			Console.WriteLine(builder.ToString(0, builder.Length - separator.Length));

			builder.Clear();
		}
	}
}
