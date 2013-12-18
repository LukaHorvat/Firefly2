using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Firefly2.Components;
using Firefly2.Messages;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Physics
{
	public class PhysicsComponent : Component, ITakesMessage<UpdateMessage>,
											ITakesMessage<ComponentCollectionChanged>
	{
		private Body body;
		private List<Fixture> fixtures;
		private double density = 1;
		private bool updateTransform = false;

		public PhysicalSettings WorldSettings;
		public double Density
		{
			get { return density; }
			set { density = value; UpdateBody(); }
		}

		public PhysicsComponent() : this(PhysicalSettings.Default) { }

		public PhysicsComponent(PhysicalSettings settings)
		{
			WorldSettings = settings;
		}

		public void ApplyForce(Vector2d force, Vector2d origin)
		{
			if (body != null)
			{
				body.ApplyForce(VectorConversion.Convert(force), VectorConversion.Convert(origin * WorldSettings.MetersPerUnit));
			}
		}

		public void AddJoint(Entity secondBody, Vector2d selfAnchor, Vector2d foreignAnchor)
		{
			var phys = secondBody.GetComponent<PhysicsComponent>();
			if (phys == null) throw new InvalidOperationException("Second body doesn't have a PhysicsComponent");
			WorldSettings.World.AddJoint(
				new RevoluteJoint(
					body,
					phys.body,
					VectorConversion.Convert(selfAnchor * WorldSettings.MetersPerUnit),
					VectorConversion.Convert(foreignAnchor * WorldSettings.MetersPerUnit)
				)
			);
		}

		public void TakeMessage(ComponentCollectionChanged msg)
		{
			if (msg.Target is GeometryComponent || msg.Target is TransformComponent)
			{
				if (msg.Type == ComponentCollectionChanged.ChangeType.Add)
				{
					UpdateBody();
					updateTransform = true;
				}
				else
				{
					ClearBody();
					updateTransform = false;
					return;
				}
			}
		}

		public void TakeMessage(UpdateMessage msg)
		{
			TransformComponent transform;
			if (updateTransform)
			{
				transform = Host.GetComponent<TransformComponent>();
				transform.X = body.Position.X * WorldSettings.UnitsPerMeter;
				transform.Y = body.Position.Y * WorldSettings.UnitsPerMeter;
				transform.Rotation = body.Rotation;
			}
		}

		private void UpdateBody()
		{
			if (Host.GetComponent<GeometryComponent>() == null ||
				Host.GetComponent<TransformComponent>() == null) return;

			ClearBody();
			var geometry = Host.GetComponent<GeometryComponent>();
			var transform = Host.GetComponent<TransformComponent>();

			body = BodyFactory.CreateBody(WorldSettings.World);
			fixtures = new List<Fixture>();
			var verts = new Vertices(geometry.Polygon.Count);
			foreach (var point in geometry.Polygon)
			{
				var newPoint = point * WorldSettings.MetersPerUnit;
				verts.Add(VectorConversion.Convert(newPoint));
			}
			var convexPolys = Triangulate.ConvexPartition(verts, TriangulationAlgorithm.Delauny);
			foreach (var convex in convexPolys)
			{
				var shape = new PolygonShape(convex, (float)density);
				fixtures.Add(body.CreateFixture(shape));
			}
			body.SetTransform(
				new Microsoft.Xna.Framework.Vector2(
					(float)(transform.X * WorldSettings.MetersPerUnit),
					(float)(transform.Y * WorldSettings.MetersPerUnit)),
				(float)transform.Rotation
			);
			body.BodyType = BodyType.Dynamic;
		}

		private void ClearBody()
		{
			if (body == null) return;
			foreach (var fixture in fixtures)
			{
				body.DestroyFixture(fixture);
			}
			WorldSettings.World.RemoveBody(body);
		}
	}
}
