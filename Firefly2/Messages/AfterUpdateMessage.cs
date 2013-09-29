using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class AfterUpdateMessage
	{
		public double DeltaTime;

		public AfterUpdateMessage(double dt)
		{
			DeltaTime = dt;
		}
	}
}
