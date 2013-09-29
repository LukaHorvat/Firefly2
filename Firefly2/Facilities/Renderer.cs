using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class Renderer
	{
		private Dictionary<string, Layer> layers;

		public Renderer()
		{
			layers = new Dictionary<string, Layer>();
		}

		public Layer GetLayer(string name)
		{
			Layer layer;
			if (!layers.TryGetValue(name, out layer))
			{
				layer = new Layer();
				layers[name] = layer;
			}
			return layer;
		}

		public void ProcessRenderBuffer(RenderBufferComponent buffer)
		{
			GetLayer("default").ModifyOrAdd(buffer);
		}
	}
}
