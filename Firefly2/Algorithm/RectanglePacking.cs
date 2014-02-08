using Firefly2.Functional.RectanglePacking;
using Firefly2.Geometry;
using Firefly2.Utility;
using Microsoft.FSharp.Collections;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Algorithm
{
	public class PackedRectangle
	{
		public AARectangle Rectangle;

		public PackedRectangle(AARectangle rect, int id)
		{
			Rectangle = rect;
			Id = id;
		}

		public int Id { get; set; }
	}

	internal class FSharpRectangle : IRectangle
	{
		internal PackedRectangle PackedRect;

		public FSharpRectangle(PackedRectangle rect)
		{
			this.PackedRect = rect;
		}

		public double Width
		{
			get { return PackedRect.Rectangle.Size.X; }
		}

		public double Height
		{
			get { return PackedRect.Rectangle.Size.Y; }
		}

		public double X
		{
			get { return PackedRect.Rectangle.Position.X; }
			set { PackedRect.Rectangle.Position.X = value; }
		}

		public double Y
		{
			get { return PackedRect.Rectangle.Position.Y; }
			set { PackedRect.Rectangle.Position.Y = value; }
		}

		public int Id
		{
			get { return PackedRect.Id; }
			set { PackedRect.Id = value; }
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

		public static List<PackedRectangle> PackRectanglesFSharp(List<PackedRectangle> rectangles)
		{
			var newRects = new List<PackedRectangle>();
			rectangles.ForEach(rect =>
				newRects.Add(new PackedRectangle(new AARectangle(Vector2d.Zero, rect.Rectangle.Size), rect.Id))
			);
			PackRectanglesModifyFSharp(newRects);

			return newRects;
		}

		public static void PackRectanglesModifyFSharp(List<PackedRectangle> rectangles)
		{
			var list =
			ListModule.OfSeq<IRectangle>
			(
				rectangles.Select(r => new FSharpRectangle(r))
			);
			Packer.packRectangles(list);
		}

		/// <summary>
		/// Packs the rectangles into a large rectangle so that it's relatively densely packed.
		/// Modifies the Position property of the input rectangles.
		/// </summary>
		/// <param name="rectangles"></param>
		/// <returns></returns>
		public static void PackRectanglesModify(List<PackedRectangle> rectangles)
		{
			#region Unfinished algorithm
			//rectangles.Sort(Comparing.CreateComparer<PackedRectangle, double>(rect =>
			//	rect.Rectangle.Size.X * rect.Rectangle.Size.Y
			//));
			//var possibleSpots = new LinkedList<LinkedMatrixNode<AARectangle>>();
			//var topLeft = new AARectangle(Vector2d.Zero, new Vector2d(double.MaxValue, double.MaxValue));
			//var matrix = new LinkedMatrix<AARectangle>();
			//matrix.AddRowToFront();
			//matrix.AddColumnToFront();
			//matrix.TopLeft.Value = topLeft;
			//possibleSpots.AddLast(matrix.TopLeft.CurrentNode);

			//for (int i = 0; i < rectangles.Count; ++i)
			//{
			//	var rect = rectangles[i].Rectangle;
			//	double minArea = double.MaxValue;
			//	LinkedMatrixNode<AARectangle> winningSpot = null;
			//	for (var spot = possibleSpots.First; spot != null; spot = spot.Next)
			//	{
			//		var spotRect = spot.Value.Value;
			//		if (spotRect.Size.X >= rect.Size.X &&
			//			spotRect.Size.Y >= rect.Size.Y)
			//		{
			//			var area = (spotRect.Position.X + rect.Size.X) *
			//				(spotRect.Position.Y + rect.Size.Y);
			//			if (area < minArea)
			//			{
			//				minArea = area;
			//				winningSpot = spot.Value;
			//			}
			//		}
			//	}
			//	if (winningSpot.Value.Size != rect.Size)
			//	{
			//		winningSpot.Value.Size = rect.Size;
			//		var newRightSpot = new AARectangle
			//		(
			//			winningSpot.Value.Position + new Vector2d(winningSpot.Value.Size.X, 0),
			//			winningSpot.Value.Size - new Vector2d(winningSpot.Value.Size.X, 0)
			//		);
			//		var newBottomSpot = new AARectangle
			//		(
			//			winningSpot.Value.Position + new Vector2d(0, winningSpot.Value.Size.Y),
			//			winningSpot.Value.Size - new Vector2d(0, winningSpot.Value.Size.Y)
			//		);
			//		var column = matrix.AddColumnRight(winningSpot);
			//		var row = matrix.AddRowBelow(winningSpot);
			//		var rowView = matrix.GetRowLeftView(row);
			//		for (rowView = matrix.GetRowLeftView(row); !rowView.IsOnRightBorder; rowView.MoveRight())
			//		{
			//			rowView.CurrentNode.Up.Value.
			//		}

			//		possibleSpots.Remove(winningSpot);
			//	}
			//}
			#endregion
			rectangles.Sort
			(
				Comparing.CreateComparer<PackedRectangle, double>
				(
					rect => rect.Rectangle.Size.X * rect.Rectangle.Size.Y
				)
			);
			rectangles.Reverse();
			var possibleSpots = new List<AARectangle>();
			possibleSpots.Add
			(
				new AARectangle
				(
					new Vector2d(0, 0),
					new Vector2d(double.MaxValue, double.MaxValue)
				)
			);
			var placedRectangles = new List<AARectangle>();
			foreach (var rect in rectangles)
			{
				possibleSpots.Sort
				(
					Comparing.CreateComparer<AARectangle, double>
					(spot =>
						Math.Max
						(
							spot.Position.X + rect.Rectangle.Size.X,
							spot.Position.Y + rect.Rectangle.Size.Y
						)
					)
				);
				var bestSpot = possibleSpots.First
				(spot =>
					spot.Size.X >= rect.Rectangle.Size.X &&
					spot.Size.Y >= rect.Rectangle.Size.Y
				);
				rect.Rectangle.Position = bestSpot.Position;
				placedRectangles.Add(rect.Rectangle);
				possibleSpots.Remove(bestSpot);
				var toRemove = new List<AARectangle>();
				foreach (var spot in possibleSpots)
				{
					FitRectangle(spot, rect.Rectangle);
					if (spot.Size.X <= 0 || spot.Size.Y <= 0)
					{
						toRemove.Add(spot);
					}
				}
				toRemove.ForEach(x => possibleSpots.Remove(x));
				possibleSpots.Add
				(
					new AARectangle
					(
						rect.Rectangle.Position.X + rect.Rectangle.Size.X,
						rect.Rectangle.Position.Y,
						bestSpot.Size.X - rect.Rectangle.Size.X,
						bestSpot.Size.Y
					)
				);
				possibleSpots.Add
				(
					new AARectangle
					(
						rect.Rectangle.Position.X,
						rect.Rectangle.Position.Y + rect.Rectangle.Size.Y,
						bestSpot.Size.X,
						bestSpot.Size.Y - rect.Rectangle.Size.Y
					)
				);
			}
		}

		private static void FitRectangle(AARectangle toFit, AARectangle other)
		{
			if (other.Position.X >= toFit.Position.X &&
				IntervalsIntersect
				(
					toFit.Position.Y,
					toFit.Position.Y + toFit.Size.Y,
					other.Position.Y,
					other.Position.Y + other.Size.Y
				))
			{
				toFit.Size.X = Math.Min(other.Position.X - toFit.Position.X, toFit.Size.X);
			}
			if (other.Position.Y >= toFit.Position.Y &&
				IntervalsIntersect
				(
					toFit.Position.X,
					toFit.Position.X + toFit.Size.X,
					other.Position.X,
					other.Position.X + other.Size.X
				))
			{
				toFit.Size.Y = Math.Min(other.Position.Y - toFit.Position.Y, toFit.Size.Y);
			}
		}

		private static bool IntervalsIntersect(double x1, double y1, double x2, double y2)
		{
			return Math.Min(y1 - x2, y2 - x1) > 0;
		}

		enum RayDirection
		{
			Up, Down, Left, Right
		}
		private static AARectangle Ray(Vector2d start, RayDirection direction, List<AARectangle> rectangles)
		{
			if (direction == RayDirection.Left)
			{
				return rectangles.Where
				(AA =>
					AA.Position.Y <= start.Y && AA.Position.Y + AA.Size.Y >= start.Y &&
					AA.Position.X <= start.X
				).OrderBy
				(AA =>
					AA.Position.X
				).LastOrDefault();
			}
			else if (direction == RayDirection.Right)
			{
				return rectangles.Where
				(AA =>
					AA.Position.Y <= start.Y && AA.Position.Y + AA.Size.Y >= start.Y &&
					AA.Position.X >= start.X
				).OrderBy
				(AA =>
					AA.Position.X
				).FirstOrDefault();
			}
			else if (direction == RayDirection.Up)
			{
				return rectangles.Where
				(AA =>
					AA.Position.X <= start.X && AA.Position.X + AA.Size.X >= start.X &&
					AA.Position.Y <= start.Y
				).OrderBy
				(AA =>
					AA.Position.Y
				).LastOrDefault();
			}
			else //Down
			{
				return rectangles.Where
				(AA =>
					AA.Position.X <= start.X && AA.Position.X + AA.Size.X >= start.X &&
					AA.Position.Y >= start.Y
				).OrderBy
				(AA =>
					AA.Position.Y
				).FirstOrDefault();
			}
		}
	}
}
