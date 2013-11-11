using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
	public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
	{
		foreach (var element in list) action(element);
	}

	/// <summary>
	/// Removes entries that satisfy a condition. Returns true if any entry was removed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="func"></param>
	/// <returns></returns>
	public static bool RemoveWhere<T>(this ICollection<T> list, Func<T, bool> func)
	{
		bool ret = false;
		list.Where(func).ToList().ForEach(element =>
		{
			ret |= list.Remove(element);
		});
		return ret;
	}

	public static bool IsAlive<T>(this WeakReference<T> reference) where T : class
	{
		T obj;
		return reference.TryGetTarget(out obj);
	}

	public static T GetTarget<T>(this WeakReference<T> reference) where T : class
	{
		T obj;
		reference.TryGetTarget(out obj);
		return obj;
	}
}
