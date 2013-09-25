using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2
{
	public interface IMessageResponder<Message, Response>
	{
		Response HandleMessage(Message msg);
	}
}
