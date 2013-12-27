using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firefly2.Physics;
using OpenTK;

namespace Firefly2.Physics.PhysicsObjects
{
	public class RevoluteJoint
	{
		private FarseerPhysics.Dynamics.Joints.RevoluteJoint joint;
		private PhysicalSettings settings;

		
		public Vector2d LocalAnchorA 
		{ 
			get 
			{ 
				return VectorConversion.Convert(joint.LocalAnchorA * settings.UnitsPerMeter);
			}
			set
			{
				joint.LocalAnchorA = VectorConversion.Convert(value * settings.MetersPerUnit);
			}
		}
					
		public Vector2d LocalAnchorB 
		{ 
			get 
			{ 
				return VectorConversion.Convert(joint.LocalAnchorB * settings.UnitsPerMeter);
			}
			set
			{
				joint.LocalAnchorB = VectorConversion.Convert(value * settings.MetersPerUnit);
			}
		}
					
		public Vector2d WorldAnchorA 
		{ 
			get 
			{ 
				return VectorConversion.Convert(joint.WorldAnchorA * settings.UnitsPerMeter);
			}
			set
			{
				joint.WorldAnchorA = VectorConversion.Convert(value * settings.MetersPerUnit);
			}
		}
					
		public Vector2d WorldAnchorB 
		{ 
			get 
			{ 
				return VectorConversion.Convert(joint.WorldAnchorB * settings.UnitsPerMeter);
			}
			set
			{
				joint.WorldAnchorB = VectorConversion.Convert(value * settings.MetersPerUnit);
			}
		}
					
		public double ReferenceAngle
		{
			get
			{
				return joint.ReferenceAngle;
			}
			set
			{
				joint.ReferenceAngle = (float)value;
			}
						
		}
					
		public double JointAngle
		{
			get
			{
				return joint.JointAngle;
			}
		}
					
		public double JointSpeed
		{
			get
			{
				return joint.JointSpeed;
			}
		}
					
		public System.Boolean LimitEnabled
		{
			get
			{
				return joint.LimitEnabled;
			}
			set
			{
				joint.LimitEnabled = value;
			}
						
		}
					
		public double LowerLimit
		{
			get
			{
				return joint.LowerLimit;
			}
			set
			{
				joint.LowerLimit = (float)value;
			}
						
		}
					
		public double UpperLimit
		{
			get
			{
				return joint.UpperLimit;
			}
			set
			{
				joint.UpperLimit = (float)value;
			}
						
		}
					
		public System.Boolean MotorEnabled
		{
			get
			{
				return joint.MotorEnabled;
			}
			set
			{
				joint.MotorEnabled = value;
			}
						
		}
					
		public double MotorSpeed
		{
			get
			{
				return joint.MotorSpeed;
			}
			set
			{
				joint.MotorSpeed = (float)value;
			}
						
		}
					
		public double MaxMotorTorque
		{
			get
			{
				return joint.MaxMotorTorque;
			}
			set
			{
				joint.MaxMotorTorque = (float)value;
			}
						
		}
					
		public double MotorImpulse
		{
			get
			{
				return joint.MotorImpulse;
			}
			set
			{
				joint.MotorImpulse = (float)value;
			}
						
		}
					
		public System.Boolean CollideConnected
		{
			get
			{
				return joint.CollideConnected;
			}
			set
			{
				joint.CollideConnected = value;
			}
						
		}
					
		public double Breakpoint
		{
			get
			{
				return joint.Breakpoint;
			}
			set
			{
				joint.Breakpoint = (float)value;
			}
						
		}
					
		public System.Boolean Enabled
		{
			get
			{
				return joint.Enabled;
			}
			set
			{
				joint.Enabled = value;
			}
						
		}
					
		public RevoluteJoint(FarseerPhysics.Dynamics.Joints.RevoluteJoint joint, PhysicalSettings settings)
		{
			this.joint = joint;
			this.settings = settings;
		}
	}
}