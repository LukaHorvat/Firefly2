using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class TransformationChanged
	{
		public static TransformationChanged Instance = new TransformationChanged();

		private TransformationChanged() { }
	}
}
