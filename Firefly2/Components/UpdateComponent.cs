using Firefly2.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class UpdateComponent : Component, 
									ITakesMessage<UpdateMessage>,
									ITakesMessage<AfterUpdateMessage>
	{
		public event Action<UpdateMessage> Update;
		public event Action<AfterUpdateMessage> AfterUpdate;

		public void TakeMessage(UpdateMessage msg)
		{
			if (Update != null) Update(msg);
		}

		public void TakeMessage(AfterUpdateMessage msg)
		{
			if (AfterUpdate != null) AfterUpdate(msg);
		}
	}
}
