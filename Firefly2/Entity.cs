using Firefly2.Messages;
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
		private Dictionary<string, Component> componentsByName;

		public Entity()
		{
			Components = new ObservableCollection<Component>();
			componentsByName = new Dictionary<string, Component>();

			Components.CollectionChanged += delegate(object target, NotifyCollectionChangedEventArgs args)
			{
				if (args.NewItems != null)
				{
					foreach (Component component in args.NewItems)
					{
						componentsByName[component.Name] = component;
						component.Host = this;
						var func = component as ITakesMessage<AddedToEntity>;
						if (func != null) func.TakeMessage(AddedToEntity.Instance);
					}
				}
				if (args.OldItems != null)
				{
					foreach (Component component in args.OldItems)
					{
						componentsByName.Remove(component.Name);
						component.Host = null;
					}
				}
			};
		}

		public Component this[string name]
		{
			get
			{
				Component comp;
				if (componentsByName.TryGetValue(name, out comp)) return comp;
				return null;
			}
			private set { }
		}

		public T GetComponent<T>() where T : Component 
		{
			return this[typeof(T).Name] as T;
		}

		public void SendMessage<T>(T message)
		{
			for (int i = 0; i < Components.Count; ++i)
			{
				var func = Components[i] as ITakesMessage<T>;
				if (func != null) func.TakeMessage(message);
			}
		}
	}
}
