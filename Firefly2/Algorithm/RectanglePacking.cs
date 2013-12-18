using Firefly2.Geometry;
using Firefly2.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Algorithm
{
	public class PackedRectangle
	{
		public AARectangle Rectangle;
		public int Id;

		public PackedRectangle(AARectangle rect, int id)
		{
			Rectangle = rect;
			Id = id;
		}
	}

	public static class RectanglePacking
	{
		/// <summary>
		/// Packs the rectangles into a large rectangle so that it's relatively densely packed.
		/// Returns a list of new rectangles and doesn't modify the input ones
		/// </summary>
		/// <param name="rectangles"></param>
		/// <returns></returns>
		public static List<PackedRectangle> PackRectangles(List<PackedRectangle> rectangles)
		{
			var list = new List<PackedRectangle>();
			rectangles.ForEach(rect =>
				list.Add(new PackedRectangle(new AARectangle(Vector2d.Zero, rect.Rectangle.Size), rect.Id))
			);
			PackRectanglesModify(list);
			return list;
		}

		/// <summary>
		/// Packs the rectangles into a large rectangle so that it's relatively densely packed.
		/// Modifies the Position property of the input rectangles.
		/// </summary>
		/// <param name="rectangles"></param>
		/// <returns></returns>
		public static void PackRectanglesModify(List<PackedRectangle> rectangles)
		{
			rectangles.Sort(Comparing.CreateComparer<PackedRectangle, double>(rect =>
				rect.Rectangle.Size.X * rect.Rectangle.Size.Y
			));
			var possibleSpots = new LinkedList<LinkedMatrixNode<AARectangle>>();
			var topLeft = new AARectangle(Vector2d.Zero, new Vector2d(double.MaxValue, double.MaxValue));
			var matrix = new LinkedMatrix<AARectangle>();
			matrix.AddRowToFront();
			matrix.AddColumnToFront();
			matrix.TopLeft.Value = topLeft;
			possibleSpots.AddLast(matrix.TopLeft.CurrentNode);

			for (int i = 0; i < rectangles.Count; ++i)
			{
				var rect = rectangles[i].Rectangle;
				double minArea = double.MaxValue;
				LinkedMatrixNode<AARectangle> winningSpot = null;
				for (var spot = possibleSpots.First; spot != null; spot = spot.Next)
				{
					var spotRect = spot.Value.Value;
					if (spotRect.Size.X >= rect.Size.X &&
						spotRect.Size.Y >= rect.Size.Y)
					{
						var area = (spotRect.Position.X + rect.Size.X) *
							(spotRect.Position.Y + rect.Size.Y);
						if (area < minArea)
						{
							minArea = area;
							winningSpot = spot.Value;
						}
					}
				}
				if (winningSpot.Value.Size != rect.Size)
				{
					var column = matrix.AddColumnRight(winningSpot);
					var row = matrix.AddRowBelow(winningSpot);
					var rowView = matrix.GetRowLeftView(row);
					for (rowView = matrix.GetRowLeftView(row); !rowView.IsOnRightBorder; rowView.MoveRight())
					{

					}

					possibleSpots.Remove(winningSpot);
				}
			}
		}
	}
}
