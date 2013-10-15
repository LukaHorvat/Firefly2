using Firefly2;
using Firefly2.Components;
using Firefly2.Facilities;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildProject
{
	class Stick : Entity
	{
		public string Name;

		[Shorthand]
		public TransformComponent Transform { get; set; }

		public Stick(Stage stage, string name)
		{
			Name = name;
			Components.Add(new GeometryComponent());
			Components.Add(new ShapeColorComponent());
			Components.Add(new TransformComponent(stage.Renderer));
			Components.Add(new RenderBufferComponent(stage.Renderer));
			Components.Add(new TreeNodeComponent());
			Components.Add(new UpdateComponent());
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(0, 5));
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(100, 5));
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(100, -5));
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(0, -5));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(1, 0, 0, 1));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(0, 1, 0, 1));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(1, 1, 0, 1));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(0, 0, 1, 1));
		}
	}
}
