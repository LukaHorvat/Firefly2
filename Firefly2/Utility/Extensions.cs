using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
	public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
	{
		foreach (var element in list) action(element);
	}

	public static void ForEach<T>(this IList<T> list, Action<T, int> action)
	{
		for (int i = 0; i < list.Count; ++i) action(list[i], i);
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

	public static System.Drawing.Point ToDrawingPoint(this OpenTK.Vector2d vec)
	{
		return new System.Drawing.Point((int)vec.X, (int)vec.Y);
	}

	public static void AddMany<T>(this ObservableCollection<T> collection, params T[] elements)
	{
		foreach (var element in elements) collection.Add(element);
	}
}
