﻿using Firefly2;
using Firefly2.Components;
using Firefly2.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildProject
{
	class Rectangle : Entity
	{
		[Shorthand]
		public TransformComponent Transform { get; set; }

		public Rectangle(float width, float height, Vector4 color)
		{
			AddComponent<RenderBufferComponent>();
			AddComponent<TransformComponent>();
			AddComponent<TreeNodeComponent>();
			AddComponent(new GeometryComponent
			{
				new Vector2d(-width / 2, -height / 2), 
				new Vector2d( width / 2, -height / 2),
				new Vector2d( width / 2,  height / 2),
				new Vector2d(-height / 2, height / 2)
			});
			AddComponent(new ShapeColorComponent
			{
				color, color, color, color
			});
		}
	}
}
