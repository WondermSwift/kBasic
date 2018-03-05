using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kBasic.Core;

namespace kBasic.Maths
{
	/// <summary>
	/// A visualizer for the Curve class.
	/// </summary>
	[ExecuteInEditMode]
	public class CurveVisualizer : MonoBehaviour 
	{
		// ----------------------------------------
		// Properties
		
		// Curve Parameters
		[SerializeField] private Curve.CurveType m_CurveType = Curve.CurveType.Sine;
		[SerializeField] private int m_PointCount = 64;
		[SerializeField] private float m_Height = 1.0f;
		[SerializeField] private float m_Distance = 1.0f;
		[SerializeField] private float m_Offset = 0.0f;
		[SerializeField] private int m_Segments = 1;
		[SerializeField] private bool m_ContinueStraight = false;
		[SerializeField] private float m_ContinueDistance = 1.0f;
		[SerializeField] private bool m_Collisions = false;
		[SerializeField] private LayerMask m_CollisionLayers;

		// Debug Options
		[SerializeField] private bool m_ShowDebug = false;
		[SerializeField] private bool m_ShowRulers = false;
		[SerializeField] private bool m_ShowCollision = false;

		// ----------------------------------------
		// Public API

		/// <summary>
        /// Type of mathmatical curve to use.
		/// </summary>
		public Curve.CurveType curveType 
		{ 
			get { return m_CurveType; } 
			set { m_CurveType = value; }
		}

		/// <summary>
        /// Amount of points used when calculating the curve.
		/// </summary>
		public int pointCount 
		{ 
			get { return m_PointCount; } 
			set { m_PointCount = value; }
		}

		/// <summary>
        /// The maximum height of the curve.
		/// </summary>
		public float height 
		{ 
			get { return m_Height; } 
			set { m_Height = value; }
		}

		/// <summary>
        /// The maximum distance of the curve.
		/// </summary>
		public float distance 
		{ 
			get { return m_Distance; } 
			set { m_Distance = value; }
		}

		/// <summary>
        /// Offsets the starting point of the curve calculation.
		/// </summary>
		public float offset 
		{ 
			get { return m_Offset; } 
			set { m_Offset = value; }
		}

		/// <summary>
        /// Amount of curve segments to complete within the distance.
		/// </summary>
		public int segments 
		{ 
			get { return m_Segments; } 
			set { m_Segments = value; }
		}

		/// <summary>
        /// If enabled the curve will continue in a straight line after the maximum curve distance.
		/// </summary>
		public bool continueStraight 
		{ 
			get { return m_ContinueStraight; } 
			set { m_ContinueStraight = value; }
		}

		/// <summary>
        /// Distance the line should continue after the maximum curve distance.
		/// </summary>
		public float continueDistance 
		{ 
			get { return m_ContinueDistance; } 
			set { m_ContinueDistance = value; }
		}

		/// <summary>
        /// Enables collisions for the curve. Calculates a world space point at the first collision.
		/// </summary>
		public bool collisions 
		{ 
			get { return m_Collisions; } 
			set { m_Collisions = value; }
		}

		/// <summary>
        /// Defines which layers the curve should collide with.
		/// </summary>
		public int collisionLayers 
		{ 
			get { return m_CollisionLayers; } 
			set { m_CollisionLayers = value; }
		}

		/// <summary>
        /// Enables debug drawing in player and edit modes.
		/// </summary>
		public bool showDebug 
		{ 
			get { return m_ShowDebug; } 
			set { m_ShowDebug = value; }
		}

		/// <summary>
        /// Enables debug rulers for maximum curve distance and height.
		/// </summary>
		public bool showRulers 
		{ 
			get { return m_ShowRulers; } 
			set { m_ShowRulers = value; }
		}

		/// <summary>
        /// Enables debug point for collision.
		/// </summary>
		public bool showCollision 
		{ 
			get { return m_ShowCollision; } 
			set { m_ShowCollision = value; }
		}

		/// <summary>
        /// Get the active curve.
		/// </summary>
		public Curve curve 
		{ 
			get 
			{ 
				if(m_Curve == null)
					m_Curve = GetCurve();
				return m_Curve; 
			} 
		}

		// ----------------------------------------
		// Members

		private Curve m_Curve;
		private GameObject m_CurveDebug;
		private GameObject m_HitPointDebug;

		// ----------------------------------------
		// Methods

		private Curve GetCurve()
		{
			Curve curve = new Curve(m_CurveType, m_PointCount, m_Height, 
									m_Distance, m_Offset, m_ContinueStraight, 
									m_ContinueDistance, m_Segments);
			curve.SetCollisionParameters(m_Collisions, m_CollisionLayers);
			return curve;
		}

		private void Update()
		{
			m_Curve = GetCurve();

			if(m_ShowDebug)
				m_CurveDebug = DebugDrawing.DrawCurve(transform, m_Curve.GetPoints(), "Debug_Curve", segments, m_ContinueStraight, m_ShowRulers);

			if(m_Collisions)
			{
				RaycastHit hit;
				if(m_Curve.GetCollision(out hit))
				{
					if(m_ShowDebug && m_ShowCollision)
						m_HitPointDebug = DebugDrawing.DrawPoint(transform, hit.point, 0.1f, "Debug_HitPoint");
				}
				else
				{
					if(m_HitPointDebug)
						DestroyImmediate(m_HitPointDebug);
				}
			}
			
			ManageDebugObjects();
		}

		// ----------------------------------------
		// Debug Methods

		private void ManageDebugObjects()
		{
			if(!m_ShowDebug)
			{
				if(m_HitPointDebug)
					DestroyImmediate(m_HitPointDebug);
				if(m_CurveDebug)
					DestroyImmediate(m_CurveDebug);
			}
			else if(!m_Collisions || !m_ShowCollision)
			{
				if(m_HitPointDebug)
					DestroyImmediate(m_HitPointDebug);
			}
		}
	}
}
