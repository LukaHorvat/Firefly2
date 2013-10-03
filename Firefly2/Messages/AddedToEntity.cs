using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class AddedToEntity
	{
		public static AddedToEntity Instance = new AddedToEntity();

		private AddedToEntity() { }
	}
}
