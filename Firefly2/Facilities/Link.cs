using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public abstract class Link
	{
		internal abstract void SetAndCastComponent(Component component);
		internal abstract void CallGenericRelink(TreeNodeComponent node);
	}

	public class Link<T> : Link
		where T : Component
	{
		public T Component;

		internal override void SetAndCastComponent(Component component)
		{
			Component = (T)component;
		}

		internal override void CallGenericRelink(TreeNodeComponent node)
		{
			node.RestoreUplink<T>(this);
		}
	}
}
