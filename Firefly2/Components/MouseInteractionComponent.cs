using Firefly2.Messages.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class MouseInteractionComponent : Component,
							IAnswersMessage<MouseIntersectQuery, MouseIntersectAnswer>
	{
		public MouseIntersectAnswer AnswerMessage(MouseIntersectQuery msg)
		{
			throw new NotImplementedException();
		}
	}
}
