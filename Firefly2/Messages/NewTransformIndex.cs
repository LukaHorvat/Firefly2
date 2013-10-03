using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class NewTransformIndex
	{
		public short Index;

		public NewTransformIndex(short index)
		{
			Index = index;
		}
	}
}
