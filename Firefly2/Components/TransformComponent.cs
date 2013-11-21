using Firefly2.Facilities;
using Firefly2.Messages;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class TransformComponent : Component,
										ITakesMessage<AfterUpdateMessage>,
										ITakesMessage<ParentTransformChanged>,
										ITakesMessage<ComponentCollectionChanged>
	{
		private class TransformState
		{
			public double ScaleX = 1, ScaleY = 1, X = 0, Y = 0, Z = 1, Rotation = 0;

			public bool TryUpdate(TransformComponent comp)
			{
				if (ScaleX != comp.ScaleX || ScaleY != comp.ScaleY ||
					X != comp.X || Y != comp.Y || Z != comp.Z ||
					Rotation != comp.Rotation)
				{
					ScaleX = comp.ScaleX;
					ScaleY = comp.ScaleY;
					X = comp.X;
					Y = comp.Y;
					Z = comp.Z;
					Rotation = comp.Rotation;
					return true;
				}
				else return false;
			}

			public Matrix4 GenerateMatrix()
			{
				Matrix4 scale, rotation, translation, scaleRotation, total;
				scale = Matrix4.CreateScale((float)ScaleX, (float)ScaleY, 1);
				rotation = Matrix4.CreateRotationZ((float)Rotation);
				translation = Matrix4.CreateTranslation((float)X, (float)Y, (float)Z);
				Matrix4.Mult(ref scale, ref rotation, out scaleRotation);
				Matrix4.Mult(ref scaleRotation, ref translation, out total);
				return total;
			}
		}

		private TransformState lastState;
		private Matrix4 personalMatrix, lastParentMatrix;
		private TreeNodeComponent tree
		{
			get { return Host.GetComponent<TreeNodeComponent>(); }
		}
		private Uplink<TransformComponent> uplink;

		public short ObjectIndex = -1;
		public double ScaleX = 1, ScaleY = 1, X = 0, Y = 0, Z = 0, Rotation = 0;
		public Matrix4 ModelMatrix;

		public TransformComponent()
		{
			lastState = new TransformState();
			lastParentMatrix = ModelMatrix = personalMatrix = Matrix4.Identity;
		}

		public void TakeMessage(AfterUpdateMessage msg)
		{
			if (lastState.TryUpdate(this))
			{
				personalMatrix = lastState.GenerateMatrix();
				RegenerateMatrix();
			}
		}

		public void TakeMessage(ParentTransformChanged msg)
		{
			lastParentMatrix = msg.Matrix;
			RegenerateMatrix();
		}

		private void RegenerateMatrix()
		{
			Matrix4.Mult(ref personalMatrix, ref lastParentMatrix, out ModelMatrix);
			Host.SendMessage(TransformationChanged.Instance);
			if (tree != null) tree.Send(new ParentTransformChanged(ModelMatrix), TreeNodeComponent.SendRange.ImmediateChildrenOnly);
		}

		public Vector4 TransformPoint(Vector4 input)
		{
			return Vector4.Transform(input, ModelMatrix);
		}

		public void TakeMessage(ComponentCollectionChanged msg)
		{
			if (msg.Target is TreeNodeComponent)
			{
				var treeNode = msg.Target as TreeNodeComponent;
				uplink = tree.CreateUplink<TransformComponent>();
				uplink.EndpointChanged += delegate
				{
					if (uplink.Component == null) lastParentMatrix = Matrix4.Identity;
					else lastParentMatrix = uplink.Component.ModelMatrix;
					RegenerateMatrix();
				};
			}
		}
	}
}
