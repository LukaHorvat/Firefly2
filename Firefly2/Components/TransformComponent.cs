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
										ITakesMessage<NewChild>,
										ITakesMessage<NewParent>,
										ITakesMessage<RemovedFromParent>,
										ITakesMessage<StartRendering>,
										ITakesMessage<StopRendering>
	{
		private class TransformState
		{
			public double ScaleX = 1, ScaleY = 1, X = 0, Y = 0, Rotation = 0;

			public bool TryUpdate(TransformComponent comp)
			{
				if (ScaleX != comp.ScaleX || ScaleY != comp.ScaleY ||
					X != comp.X || Y != comp.Y ||
					Rotation != comp.Rotation)
				{
					ScaleX = comp.ScaleX;
					ScaleY = comp.ScaleY;
					X = comp.X;
					Y = comp.Y;
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
				translation = Matrix4.CreateTranslation((float)X, (float)Y, 0);
				Matrix4.Mult(ref scale, ref rotation, out scaleRotation);
				Matrix4.Mult(ref scaleRotation, ref translation, out total);
				return total;
			}
		}

		private bool rendering = false;
		private TransformState lastState;
		private Matrix4 personalMatrix, lastParentMatrix;
		private TreeNodeComponent tree
		{
			get { return Host.GetComponent<TreeNodeComponent>(); }
		}

		public short ObjectIndex = -1;
		public double ScaleX = 1, ScaleY = 1, X = 0, Y = 0, Rotation = 0;
		public Matrix4 ModelMatrix;
		public Renderer Renderer;

		public TransformComponent(Renderer renderer)
		{
			Renderer = renderer;
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
			//ModelMatrix = lastParentMatrix * personalMatrix;
			Renderer.ProcessTransform(this);
			if (tree != null) tree.Send(new ParentTransformChanged(ModelMatrix), TreeNodeComponent.SendRange.ImmediateChildrenOnly);
		}

		public void TakeMessage(NewChild msg)
		{
			msg.Child.Host.SendMessage(new ParentTransformChanged(ModelMatrix));
		}

		public void TakeMessage(RemovedFromParent msg)
		{
			lastParentMatrix = Matrix4.Identity;
			RegenerateMatrix();
		}

		public void TakeMessage(StartRendering msg)
		{
			if (!rendering)
			{
				rendering = true;
				var index = Renderer.ProcessTransform(this);
				if (ObjectIndex != index)
				{
					ObjectIndex = index;
					Host.SendMessage(new NewTransformIndex(index));
				}
			}
		}

		public void TakeMessage(StopRendering msg)
		{
			if (rendering)
			{
				rendering = false;
				Renderer.RemoveTransform(this);
				ObjectIndex = -1;
			}
		}

		public void TakeMessage(NewParent msg)
		{
			if (tree.Parent.Host is Stage) Host.SendMessage(StartRendering.Instance);
		}
	}
}
