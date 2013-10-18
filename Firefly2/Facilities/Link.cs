using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public abstract class Uplink
	{
		internal abstract void CastAndSetComponent(Component component);
		internal abstract void CallGenericRelink(TreeNodeComponent node);
	}

	public class Uplink<T> : Uplink
		where T : Component
	{
		public T Component;

		internal override void CastAndSetComponent(Component component)
		{
			Component = (T)component;
		}

		internal override void CallGenericRelink(TreeNodeComponent node)
		{
			node.RestoreUplink(this);
		}
	}

	public abstract class Downlink
	{
		internal abstract void CastAndAddComponent(Component component);
		internal abstract void CastAndRemoveComponent(Component component);
		internal abstract void CallGenericRelink(TreeNodeComponent node);
		internal abstract void RemoveMatchingComponent(TreeNodeComponent node);
	}

	public class Downlink<T> : Downlink
		where T : Component
	{
		public LinkedList<T> Components;

		public Downlink()
		{
			Components = new LinkedList<T>();
		}

		internal override void CastAndAddComponent(Component component)
		{
			Components.AddLast((T)component);
		}

		internal override void CastAndRemoveComponent(Component component)
		{
			Components.Remove((T)component);
		}

		internal override void CallGenericRelink(TreeNodeComponent node)
		{
			node.RestoreDownlink(this);
		}

		internal override void RemoveMatchingComponent(TreeNodeComponent node)
		{
			Components.Remove(node.Host.GetComponent<T>());
		}
	}
}
