using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Algorithm
{
	public static class Comparing
	{
		public static Comparison<TType> CreateComparer<TType, TComparable>(Func<TType, TComparable> func)
			where TComparable : IComparable
		{
			return (first, second) =>
			{
				if (first == null && second == null) return 0;
				if (first == null && second != null) return 1;
				if (first != null && second == null) return -1;
				TComparable firstValue = func(first), secondValue = func(second);
				if (firstValue == null && secondValue == null) return 0;
				if (firstValue == null && secondValue != null) return 1;
				if (firstValue != null && secondValue == null) return -1;
				return firstValue.CompareTo(secondValue);
			};
		}
	}
}
