using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Geometry
{
	public class Triangle<TVector>
	{
		public TVector[] Points;

		public TVector A { get { return Points[0]; } }
		public TVector B { get { return Points[1]; } }
		public TVector C { get { return Points[2]; } }

		public Triangle(TVector a, TVector b, TVector c)
		{
			Points = new[] { a, b, c };
		}
	}
}
