using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Exceptions
{
	public class LayerFullException : Exception
	{
		public LayerFullException()
			: base("Layers is full and objects cannot be added to it") { }
	}
}
