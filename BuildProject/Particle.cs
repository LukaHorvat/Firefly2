using Firefly2;
using Firefly2.Components;
using Firefly2.Messages;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildProject
{
	class Particle : Entity
	{
		private TransformComponent transform;

		public Particle(Stage stage)
		{
			var rand = new Random();
			AddComponent<GeometryComponent>();
			AddComponent<ShapeColorComponent>();
			Add(transform = new TransformComponent(stage.Renderer));
			Add(new RenderBufferComponent(stage.Renderer));
			AddComponent<TreeNodeComponent>();
			AddComponent<UpdateComponent>();
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(rand.Next(-5, 0), rand.Next(-5, 0)));
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(rand.Next(0, 5), rand.Next(-5, 0)));
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(rand.Next(0, 5), rand.Next(0, 5)));
			GetComponent<GeometryComponent>().Polygon.Add(new Vector2d(rand.Next(-5, 0), rand.Next(0, 5)));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(1, 0, 0, 1));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(0, 1, 0, 1));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(1, 1, 0, 1));
			GetComponent<ShapeColorComponent>().Colors.Add(new Vector4(0, 0, 1, 1));
			GetComponent<TransformComponent>().Rotation = Math.PI / 8;
			//GetComponent<UpdateComponent>().Update += delegate(UpdateMessage msg)
			//{
			//	transform.Rotation += 6F * msg.DeltaTime;
			//};
		}
	}
}
