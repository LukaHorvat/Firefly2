using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2
{
	public interface IAnswersMessage<Message, Response>
	{
		Response AnswerMessage(Message msg);
	}
}
