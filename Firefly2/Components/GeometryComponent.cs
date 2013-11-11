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
	public class GeometryComponent : Component, IEnumerable<Vector2d>
	{
		public ObservableCollection<Vector2d> Polygon;

		public GeometryComponent()
		{
			Polygon = new ObservableCollection<Vector2d>();
			Polygon.CollectionChanged += delegate
			{
				if (Host != null) Host.SendMessage(GeometryChanged.Instance);
			};
		}

		public bool IntersectsPoint(Vector2d point)
		{
			int linesToTheRight = 0;
			for (int i = 0; i < Polygon.Count; ++i)
			{
				int j = (i + 1) % Polygon.Count;
				if (Polygon[i].Y > point.Y && Polygon[j].Y > point.Y) continue;
				if (Polygon[i].Y < point.Y && Polygon[j].Y < point.Y) continue;
				if (Polygon[i].X < point.X && Polygon[j].X < point.X) continue;
				if (Polygon[i].X == Polygon[j].X && Polygon[j].X == point.X) continue;
				if (Polygon[i].X > point.X && Polygon[j].X > point.X) linesToTheRight++;
				else
				{
					Vector2d left = Polygon[i], right = Polygon[j];
					if (Polygon[i].X > Polygon[j].X)
					{
						var temp = left;
						left = right;
						right = temp;
					}
					double k = (right.Y - left.Y) / (right.X - left.X);
					double lineY = k * (point.X - left.X) + left.Y;
					if (lineY < point.Y)
					{
						if (left.Y > right.Y) continue;
						else linesToTheRight++;
					}
					else
					{
						if (left.Y > right.Y) linesToTheRight++;
						else continue;
					}
				}
			}
			if (linesToTheRight % 2 == 1) return true;
			else return false;
		}

		public void Add(Vector2d vector)
		{
			Polygon.Add(vector);
		}

		public IEnumerator<Vector2d> GetEnumerator()
		{
			return Polygon.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Polygon.GetEnumerator();
		}
	}
}
