using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Utility
{
	public class LinkedMatrix<T> where T : class
	{
		private LinkedList<int> rows, columns;
		private MatrixLookup<T> lookup;
		private int rowId = 0, columnId = 0;

		public LinkedMatrixView<T> TopLeft
		{
			get
			{
				if (rows.Count == 0 || columns.Count == 0) return null;
				return new LinkedMatrixView<T>(lookup, rows.First, columns.First);
			}
		}

		public LinkedMatrix()
		{
			rows = new LinkedList<int>();
			columns = new LinkedList<int>();
			lookup = new MatrixLookup<T>();
		}

		public LinkedListNode<int> AppendRow()
		{
			return rows.AddLast(rowId++);
		}

		public LinkedListNode<int> AppendColumn()
		{
			return columns.AddLast(columnId++);
		}

		public LinkedListNode<int> AddRowToFront()
		{
			return rows.AddFirst(rowId++);
		}

		public LinkedListNode<int> AddColumnToFront()
		{
			return rows.AddFirst(columnId++);
		}

		/// <summary>
		/// Add a new row above the given view.
		/// </summary>
		/// <param name="view"></param>
		public LinkedListNode<int> AddRowAbove(LinkedMatrixView<T> view)
		{
			return rows.AddBefore(view.Row, rowId++);
		}

		/// <summary>
		/// Add a new row below the given view.
		/// </summary>
		/// <param name="view"></param>
		public LinkedListNode<int> AddRowBelow(LinkedMatrixView<T> view)
		{
			return rows.AddAfter(view.Row, rowId++);
		}

		/// <summary>
		/// Add a column to the left of the given view.
		/// </summary>
		/// <param name="view"></param>
		public LinkedListNode<int> AddColumnLeft(LinkedMatrixView<T> view)
		{
			return columns.AddBefore(view.Column, columnId++);
		}

		/// <summary>
		/// Add a column to the right of the given view.
		/// </summary>
		/// <param name="view"></param>
		public LinkedListNode<int> AddColumnRight(LinkedMatrixView<T> view)
		{
			return columns.AddAfter(view.Column, columnId++);
		}

		/// <summary>
		/// Add a new row above the given node.
		/// </summary>
		/// <param name="node"></param>
		public LinkedListNode<int> AddRowAbove(LinkedMatrixNode<T> node)
		{
			return rows.AddBefore(node.Row, rowId++);
		}

		/// <summary>
		/// Add a new row below the given node.
		/// </summary>
		/// <param name="node"></param>
		public LinkedListNode<int> AddRowBelow(LinkedMatrixNode<T> node)
		{
			return rows.AddAfter(node.Row, rowId++);
		}

		/// <summary>
		/// Add a column to the left of the given node.
		/// </summary>
		/// <param name="node"></param>
		public LinkedListNode<int> AddColumnLeft(LinkedMatrixNode<T> node)
		{
			return columns.AddBefore(node.Column, columnId++);
		}

		/// <summary>
		/// Add a column to the right of the given node.
		/// </summary>
		/// <param name="node"></param>
		public LinkedListNode<int> AddColumnRight(LinkedMatrixNode<T> node)
		{
			return columns.AddAfter(node.Column, columnId++);
		}

		/// <summary>
		/// Gets a view that points to the leftmost node in the specified row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public LinkedMatrixView<T> GetRowLeftView(LinkedListNode<int> row)
		{
			if (row == null || columns.Count == 0) return null;
			return new LinkedMatrixView<T>(lookup, row, columns.First);
		}

		/// <summary>
		/// Gets a view that points to the rightmost node in the specified row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public LinkedMatrixView<T> GetRowRightView(LinkedListNode<int> row)
		{
			if (row == null || columns.Count == 0) return null;
			return new LinkedMatrixView<T>(lookup, row, columns.Last);
		}

		/// <summary>
		/// Gets a view that points to the top of the specified column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public LinkedMatrixView<T> GetColumnTopView(LinkedListNode<int> column)
		{
			if (column == null || rows.Count == 0) return null;
			return new LinkedMatrixView<T>(lookup, column, rows.First);
		}

		/// <summary>
		/// Gets a view that points to the bottom of the specified column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public LinkedMatrixView<T> GetColumnBottomView(LinkedListNode<int> column)
		{
			if (column == null || rows.Count == 0) return null;
			return new LinkedMatrixView<T>(lookup, column, rows.Last);
		}
	}

	internal class MatrixLookup<T1>
	{
		private Dictionary<LinkedListNode<int>, Dictionary<LinkedListNode<int>, T1>> dict;

		internal MatrixLookup()
		{
			dict = new Dictionary<LinkedListNode<int>, Dictionary<LinkedListNode<int>, T1>>();
		}

		public T1 GetNode(LinkedListNode<int> row, LinkedListNode<int> column)
		{
			if (row == null || column == null) return default(T1);
			if (dict.ContainsKey(row))
			{
				var rowDict = dict[row];
				if (rowDict.ContainsKey(column))
				{
					return rowDict[column];
				}
				return default(T1);
			}
			dict[row] = new Dictionary<LinkedListNode<int>, T1>();
			return default(T1);
		}

		public void SetNode(LinkedListNode<int> row, LinkedListNode<int> column, T1 value)
		{
			if (row == null || column == null) throw new InvalidOperationException("Cannot insert value into an empty matrix.");
			if (!dict.ContainsKey(row)) dict[row] = new Dictionary<LinkedListNode<int>, T1>();
			dict[row][column] = value;
		}
	}

	public class LinkedMatrixView<T1>
	{
		public void MoveUp()
		{
			if (Row == null) new InvalidOperationException("The matrix is empty.");
			if (Row.Previous == null) new InvalidOperationException("The view already points to the top row.");
			Row = Row.Previous;
		}
		public void MoveDown()
		{
			if (Row == null) new InvalidOperationException("The matrix is empty.");
			if (Row.Next == null) new InvalidOperationException("The view already points to the bottom row.");
			Row = Row.Next;
		}
		public void MoveLeft()
		{
			if (Column == null) new InvalidOperationException("The matrix is empty.");
			if (Column.Previous == null) new InvalidOperationException("The view already points to the leftmost column.");
			Column = Column.Previous;
		}
		public void MoveRight()
		{
			if (Column == null) new InvalidOperationException("The matrix is empty.");
			if (Column.Next == null) new InvalidOperationException("The view already points to the rightmost column.");
			Column = Column.Next;
		}

		public T1 Value
		{
			get { return dict.GetNode(Row, Column); }
			set { dict.SetNode(Row, Column, value); }
		}

		public LinkedMatrixNode<T1> CurrentNode
		{
			get { return new LinkedMatrixNode<T1>(dict, Row, Column); }
		}

		public bool IsOnLeftBorder { get { return Column == null || Column.Previous == null; } }
		public bool IsOnRightBorder { get { return Column == null || Column.Next == null; } }
		public bool IsOnTopBorder { get { return Row == null || Row.Previous == null; } }
		public bool IsOnBottomBorder { get { return Row == null || Row.Next == null; } }

		public LinkedListNode<int> Row, Column;
		private MatrixLookup<T1> dict;

		internal LinkedMatrixView(MatrixLookup<T1> dict, LinkedListNode<int> row, LinkedListNode<int> column)
		{
			this.dict = dict;
			Row = row;
			Column = column;
		}
	}

	public class LinkedMatrixNode<T1> : IEquatable<LinkedMatrixNode<T1>>
	{
		public readonly LinkedListNode<int> Row, Column;
		private MatrixLookup<T1> dict;

		public T1 Value
		{
			get { return dict.GetNode(Row, Column); }
			set { dict.SetNode(Row, Column, value); }
		}

		public LinkedMatrixView<T1> View
		{
			get { return new LinkedMatrixView<T1>(dict, Row, Column); }
		}

		public LinkedMatrixNode<T1> Left
		{
			get { return new LinkedMatrixNode<T1>(dict, Row, Column.Previous); }
		}

		public LinkedMatrixNode<T1> Right
		{
			get { return new LinkedMatrixNode<T1>(dict, Row, Column.Next); }
		}

		public LinkedMatrixNode<T1> Up
		{
			get { return new LinkedMatrixNode<T1>(dict, Row.Previous, Column); }
		}

		public LinkedMatrixNode<T1> Down
		{
			get { return new LinkedMatrixNode<T1>(dict, Row.Next, Column); }
		}

		internal LinkedMatrixNode(MatrixLookup<T1> dict, LinkedListNode<int> row, LinkedListNode<int> column)
		{
			if (row == null || column == null) throw new InvalidOperationException("Matrix node out of bounds.");
			this.Row = row;
			this.Column = column;
			this.dict = dict;
		}

		public override int GetHashCode()
		{
			return Row.GetHashCode() ^ Column.GetHashCode();
		}

		public bool Equals(LinkedMatrixNode<T1> other)
		{
			if (Row == other.Row && Column == other.Column) return true;
			return false;
		}
	}
}
