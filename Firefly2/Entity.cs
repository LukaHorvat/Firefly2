using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2
{
	public class Entity
	{
		public ObservableCollection<Component> Components;

		public Entity()
		{
			Components = new ObservableCollection<Component>();
			Components.CollectionChanged += delegate(object target, NotifyCollectionChangedEventArgs args)
			{
				
			};
		}

		public object SendMessage(object message)
		{
			foreach (var component in Components)
			{
				var res = component.ProcessMessage(message);
				if (res != null) return res;
			}
			return null;
		}
	}
}
