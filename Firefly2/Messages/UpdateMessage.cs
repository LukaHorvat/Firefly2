﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class UpdateMessage
	{
		public double DeltaTime;

		public UpdateMessage(double dt)
		{
			DeltaTime = dt;
		}
	}
}
