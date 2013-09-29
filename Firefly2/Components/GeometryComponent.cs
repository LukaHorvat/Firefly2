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
	public class GeometryComponent : Component
	{
		public ObservableCollection<Vector2d> Polygon;

		public GeometryComponent()
		{
			Polygon = new ObservableCollection<Vector2d>();
			Polygon.CollectionChanged += delegate
			{
				Host.SendMessage(GeometryChanged.Instance);
			};
		}
	}
}
