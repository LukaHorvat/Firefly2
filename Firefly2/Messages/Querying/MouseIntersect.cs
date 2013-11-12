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
		public static MouseIntersectQuery Instance = new MouseIntersectQuery();

		private MouseIntersectQuery() { }
	}

	public enum MouseIntersectAnswer
	{
		Intersects,
		DoesNotIntersect
	}
}
