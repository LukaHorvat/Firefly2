using Firefly2.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Messages
{
	public class RendererChanged
	{
		public Renderer NewRenderer;
		
		public RendererChanged(Renderer renderer)
		{
			NewRenderer = renderer;
		}
	}
}
