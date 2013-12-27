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
											ITakesMessage<ComponentCollectionChanged>,
											ITakesMessage<GeometryChanged>
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
		/// <summary>
		/// Get the location of the center of mass in local coordinates relative to this entity
		/// </summary>
		public Vector2d CenterOfMass
		{
			get
			{
				if (body == null) return Vector2d.Zero;
				return VectorConversion.Convert(body.LocalCenter * WorldSettings.UnitsPerMeter);
			}
		}
		/// <summary>
		/// Get or set the position of the body.
		/// </summary>
		public Vector2d BodyPosition
		{
			get
			{
				if (body == null) return Vector2d.Zero;
				return VectorConversion.Convert(body.Position * WorldSettings.UnitsPerMeter);
			}
			set
			{
				if (body == null) return;
				body.SetTransform(VectorConversion.Convert(value * WorldSettings.MetersPerUnit), body.Rotation);
			}
		}
		/// <summary>
		/// Get or set the rotation of the body.
		/// </summary>
		public double BodyRotation
		{
			get
			{
				if (body == null) return 0;
				return body.Rotation;
			}
			set
			{
				if (body == null) return;
				body.SetTransform(body.Position, (float)value);
			}
		}

		/// <summary>
		/// Get or set the velocity of the body.
		/// </summary>
		public Vector2d BodyVelocity
		{
			get
			{
				if (body == null) return Vector2d.Zero;
				return VectorConversion.Convert(body.LinearVelocity * WorldSettings.UnitsPerMeter);
			}
			set
			{
				if (body == null) return;
				body.LinearVelocity = VectorConversion.Convert(value * WorldSettings.MetersPerUnit);
			}
		}

		/// <summary>
		/// Get or set the angular velocity of the body.
		/// </summary>
		public double BodyAngularVelocity
		{
			get
			{
				if (body == null) return 0;
				return body.AngularVelocity;
			}
			set
			{
				if (body == null) return;
				body.AngularVelocity = (float)value;
			}
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

		public void ApplyForce(Vector2d force)
		{
			if (body != null)
			{
				body.ApplyForce(VectorConversion.Convert(force));
			}
		}

		public Vector2d GetLocal(Vector2d global)
		{
			if (body == null) return Vector2d.Zero;
			return VectorConversion.Convert(
				body.GetLocalPoint(
					VectorConversion.Convert(global * WorldSettings.MetersPerUnit) * WorldSettings.UnitsPerMeter
				)
			);
		}

		public Vector2d GetGlobal(Vector2d local)
		{
			if (body == null) return Vector2d.Zero;
			return VectorConversion.Convert(
				body.GetWorldPoint(
					VectorConversion.Convert(local * WorldSettings.MetersPerUnit)
				) * WorldSettings.UnitsPerMeter
			);
		}

		public Firefly2.Physics.PhysicsObjects.RevoluteJoint AddJoint(Entity secondBody, Vector2d selfAnchor, Vector2d foreignAnchor)
		{
			if (body == null) throw new InvalidOperationException("Body is null");
			var phys = secondBody.GetComponent<PhysicsComponent>();
			if (phys == null) throw new InvalidOperationException("Second entity doesn't have a PhysicsComponent");
			if (phys.body == null) throw new InvalidOperationException("Second PhysicsComponent doesn't have a body");
			var joint = new RevoluteJoint
			(
				body,
				phys.body,
				VectorConversion.Convert(selfAnchor * WorldSettings.MetersPerUnit),
				VectorConversion.Convert(foreignAnchor * WorldSettings.MetersPerUnit)
			)
			{
				MotorEnabled = true,
				MaxMotorTorque = 1,
				//LimitEnabled = true,
				//LowerLimit = 0,
				//UpperLimit = 0
			};
			WorldSettings.World.AddJoint(
				joint
			);
			return new Firefly2.Physics.PhysicsObjects.RevoluteJoint(joint, WorldSettings);
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

		public void TakeMessage(GeometryChanged msg)
		{
			UpdateBody();
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
				Host.GetComponent<TransformComponent>() == null ||
				Host.GetComponent<GeometryComponent>().Polygon.Count == 0) return;

			ClearBody();
			var geometry = Host.GetComponent<GeometryComponent>();
			var transform = Host.GetComponent<TransformComponent>();

			if (geometry.Polygon.Count < 3) return;

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
