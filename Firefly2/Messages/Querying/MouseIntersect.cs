using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages.Querying
{
	public class MouseIntersectQuery
	{
		public Vector2d MousePosition;

		public MouseIntersectQuery(Vector2d mousePosition)
		{
			MousePosition = mousePosition;
		}
	}

	public enum MouseIntersectAnswer
	{
		Intersects,
		DoesNotIntersect
	}
}
