using Firefly2.Facilities;
using Firefly2.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2
{
	public class Entity : IEnumerable<Component>
	{
		/// <summary>
		/// Attribute that marks a property as a shorthand to a component. Can only be added to
		/// properties whose type is a subclass of Component.
		/// </summary>
		[AttributeUsage(AttributeTargets.Property)]
		protected class Shorthand : Attribute
		{
		}

		private ObservableCollection<Component> Components;
		private Dictionary<string, Component> componentsByName;

		public static Dictionary<Type, Dictionary<string, Action<Entity, object>>> shorthandMap
			= new Dictionary<Type, Dictionary<string, Action<Entity, object>>>();

		public Entity()
		{
			//If this class hasn't already been processed, get all the shorthands it has and cache
			//setter methods for them. This is done via reflection. The shorthands are modified on
			//component add or remove
			var type = GetType();
			if (!shorthandMap.ContainsKey(type))
			{
				shorthandMap[type] = new Dictionary<string, Action<Entity, object>>();
				var shorthands = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(field => Attribute.IsDefined(field, typeof(Shorthand)));
				var helper = typeof(Entity).GetMethod("GenerateFieldSetMethod", BindingFlags.Static | BindingFlags.NonPublic);
				foreach (var shorthand in shorthands)
				{
					var method = helper.MakeGenericMethod(type, shorthand.PropertyType);
					shorthandMap[type][shorthand.PropertyType.Name] = (Action<Entity, object>)method.Invoke(null, new object[] { shorthand.GetSetMethod(true) });
				}
			}

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

						UpdateShorthand(component.GetType().Name, component);

						NotifyComponents(ComponentCollectionChanged.ChangeType.Add, component);
					}
				}
				if (args.OldItems != null)
				{
					foreach (Component component in args.OldItems)
					{
						componentsByName.Remove(component.Name);
						component.Host = null;
						var func = component as ITakesMessage<RemovedFromParent>;
						if (func != null) func.TakeMessage(RemovedFromParent.Instance);

						UpdateShorthand(component.GetType().Name, null);

						NotifyComponents(ComponentCollectionChanged.ChangeType.Remove, component);
					}
				}
			};
		}

		private void NotifyComponents(ComponentCollectionChanged.ChangeType type, Component component)
		{
			var msg = new ComponentCollectionChanged(component, type);
			var takesMessage = component as ITakesMessage<ComponentCollectionChanged>;
			for (int i = 0; i < Components.Count; ++i)
			{
				if (Components[i] != component)
				{
					var listener = Components[i] as ITakesMessage<ComponentCollectionChanged>;
					if (listener != null) listener.TakeMessage(msg);
					if (takesMessage != null)
					{
						takesMessage.TakeMessage(
							new ComponentCollectionChanged(Components[i], type));
					}
				}
			}
		}

		private void UpdateShorthand(string name, object value)
		{
			Action<Entity, object> field;
			if (shorthandMap[GetType()].TryGetValue(name, out field))
			{
				field(this, value);
			}
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

		public void AddComponent<T>() where T : Component, new()
		{
			Components.Add(new T());
		}

		/// <summary>
		/// Same as AddComponent. This method enables the use of collection initializers for entities.
		/// </summary>
		/// <param name="component"></param>
		public void Add(Component component)
		{
			Components.Add(component);
		}

		public void AddComponent(Component component)
		{
			Components.Add(component);
		}

		public void RemoveComponent(Component component)
		{
			Components.Remove(component);
		}

		public void RemoveComponent<T>() where T : Component
		{
			Components.RemoveWhere(c => c is T);
		}

		public void SendMessage<T>(T message)
		{
			for (int i = 0; i < Components.Count; ++i)
			{
				var func = Components[i] as ITakesMessage<T>;
				if (func != null) func.TakeMessage(message);
			}
		}

		private static Action<Entity, object> GenerateFieldSetMethod<TEntity, TComponent>(MethodInfo method)
			where TComponent : Component
			where TEntity : Entity
		{
			return (e, p) => ((Action<TEntity, TComponent>)Delegate.CreateDelegate(typeof(Action<TEntity, TComponent>), method))((TEntity)e, (TComponent)p);
		}

		public IEnumerator<Component> GetEnumerator()
		{
			return Components.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Components.GetEnumerator();
		}
	}
}
