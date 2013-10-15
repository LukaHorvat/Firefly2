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
	public class Entity
	{
		/// <summary>
		/// Attribute that marks a property as a shorthand to a component. Can only be added to
		/// properties whose type is a subclass of Component.
		/// </summary>
		[AttributeUsage(AttributeTargets.Property)]
		protected class Shorthand : Attribute
		{
		}

		public ObservableCollection<Component> Components;
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
				var shorthands = type.GetProperties()
					.Where(field => Attribute.IsDefined(field, typeof(Shorthand)));
				var helper = typeof(Entity).GetMethod("GenerateFieldSetMethod", BindingFlags.Static | BindingFlags.NonPublic);
				foreach (var shorthand in shorthands)
				{
					var method = helper.MakeGenericMethod(type, shorthand.PropertyType);
					shorthandMap[type][shorthand.PropertyType.Name] = (Action<Entity, object>)method.Invoke(null, new object[] { shorthand.GetSetMethod() });
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

						Action<Entity, object> field;
						if (shorthandMap[GetType()].TryGetValue(component.GetType().Name, out field))
						{
							field(this, component);
						}
					}
				}
				if (args.OldItems != null)
				{
					foreach (Component component in args.OldItems)
					{
						componentsByName.Remove(component.Name);
						component.Host = null;

						Action<Entity, object> field;
						if (shorthandMap[GetType()].TryGetValue(component.GetType().Name, out field))
						{
							field(this, null);
						}
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

		private static Action<Entity, object> GenerateFieldSetMethod<TEntity, TComponent>(MethodInfo method)
			where TComponent : Component
			where TEntity : Entity
		{
			return (e, p) => ((Action<TEntity, TComponent>)Delegate.CreateDelegate(typeof(Action<TEntity, TComponent>), method))((TEntity)e, (TComponent)p);
		}
	}
}
