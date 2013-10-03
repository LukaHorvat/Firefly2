using Firefly2.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class UpdateComponent : Component, ITakesMessage<UpdateMessage>
	{
		public event Action<UpdateMessage> Update; 

		public void TakeMessage(UpdateMessage msg)
		{
			if (Update != null) Update(msg);
		}
	}
}
