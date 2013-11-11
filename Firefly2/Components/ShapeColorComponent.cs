using Firefly2.Messages;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	/// <summary>
	/// Class that defiines the vertex colors for the geometry component
	/// </summary>
	public class ShapeColorComponent : Component, IEnumerable<Vector4>
	{
		public ObservableCollection<Vector4> Colors;

		public ShapeColorComponent()
		{
			Colors = new ObservableCollection<Vector4>();
			Colors.CollectionChanged += delegate
			{
				if (Host != null) Host.SendMessage(ShapeColorChanged.Instance);
			};
		}

		public void Add(Vector4 color)
		{
			Colors.Add(color);
		}

		public IEnumerator<Vector4> GetEnumerator()
		{
			return Colors.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Colors.GetEnumerator();
		}
	}
}
