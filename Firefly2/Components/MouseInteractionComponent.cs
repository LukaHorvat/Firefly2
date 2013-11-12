using Firefly2.Messages.Querying;
using Firefly2.Utility;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class MouseInteractionComponent : Component,
							IAnswersMessage<MouseIntersectQuery, MouseIntersectAnswer>
	{
		public MutableVector2 Mouse;
		public bool IntersectsMouse
		{
			get
			{
				return AnswerMessage(MouseIntersectQuery.Instance) == MouseIntersectAnswer.Intersects;
			}
		}

		public MouseInteractionComponent(MutableVector2 mouse)
		{
			Mouse = mouse;
		}

		public MouseIntersectAnswer AnswerMessage(MouseIntersectQuery msg)
		{
			var geometry = Host.GetComponent<GeometryComponent>();
			var transform = Host.GetComponent<TransformComponent>();
			var tree = Host.GetComponent<TreeNodeComponent>();
			if (geometry != null && transform != null)
			{
				var transformedMouse = Vector4.Transform(
					new Vector4((float)Mouse.X, (float)Mouse.Y, 0, 1),
					transform.ModelMatrix.Inverted());
				if (geometry.IntersectsPoint(new Vector2d(transformedMouse.X, transformedMouse.Y))) return MouseIntersectAnswer.Intersects;
			}
			if (tree != null)
			{
				var result = tree.Query<
					MouseIntersectQuery,
					MouseIntersectAnswer,
					MouseInteractionComponent>(msg, TreeNodeComponent.QueryRange.StopAtReceivers);
				if (result.Any(ans => ans == MouseIntersectAnswer.Intersects)) return MouseIntersectAnswer.Intersects;
			}
			return MouseIntersectAnswer.DoesNotIntersect;
		}
	}
}
