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
	public class ShapeColorComponent : Component
	{
		public ObservableCollection<Vector4> Colors;

		public ShapeColorComponent()
		{
			Colors = new ObservableCollection<Vector4>();
			Colors.CollectionChanged += delegate
			{
				Host.SendMessage(ShapeColorChanged.Instance);
			};
		}
	}
}
