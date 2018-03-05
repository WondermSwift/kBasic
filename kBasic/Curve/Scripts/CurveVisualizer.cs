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
		[SerializeField] private Curve.Properties m_Properties;

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
			get { return m_Properties.curveType; } 
			set { m_Properties.curveType = value; }
		}

		/// <summary>
        /// Amount of points used when calculating the curve.
		/// </summary>
		public int pointCount 
		{ 
			get { return m_Properties.pointCount; } 
			set { m_Properties.pointCount = value; }
		}

		/// <summary>
        /// The maximum height of the curve.
		/// </summary>
		public float height 
		{ 
			get { return m_Properties.height; } 
			set { m_Properties.height = value; }
		}

		/// <summary>
        /// The maximum distance of the curve.
		/// </summary>
		public float distance 
		{ 
			get { return m_Properties.distance; } 
			set { m_Properties.distance = value; }
		}

		/// <summary>
        /// Offsets the starting point of the curve calculation.
		/// </summary>
		public float offset 
		{ 
			get { return m_Properties.offset; } 
			set { m_Properties.offset = value; }
		}

		/// <summary>
        /// Amount of curve segments to complete within the distance.
		/// </summary>
		public int segments 
		{ 
			get { return m_Properties.segments; } 
			set { m_Properties.segments = value; }
		}

		/// <summary>
        /// If enabled the curve will continue in a straight line after the maximum curve distance.
		/// </summary>
		public bool continueStraight 
		{ 
			get { return m_Properties.continueStraight; } 
			set { m_Properties.continueStraight = value; }
		}

		/// <summary>
        /// Distance the line should continue after the maximum curve distance.
		/// </summary>
		public float continueDistance 
		{ 
			get { return m_Properties.continueDistance; } 
			set { m_Properties.continueDistance = value; }
		}

		/// <summary>
        /// Enables collisions for the curve. Calculates a world space point at the first collision.
		/// </summary>
		public bool collisions 
		{ 
			get { return m_Properties.collisions; } 
			set { m_Properties.collisions = value; }
		}

		/// <summary>
        /// Defines which layers the curve should collide with.
		/// </summary>
		public int collisionLayers 
		{ 
			get { return m_Properties.collisionLayers; } 
			set { m_Properties.collisionLayers = value; }
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
					m_Curve = NewCurve();
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

		private Curve NewCurve()
		{
			Curve curve = new Curve(m_Properties.curveType, m_Properties.pointCount, m_Properties.height, 
									m_Properties.distance, m_Properties.offset, m_Properties.continueStraight, 
									m_Properties.continueDistance, m_Properties.segments);
			curve.SetCollisionParameters(m_Properties.collisions, m_Properties.collisionLayers);
			return curve;
		}

		private void Update()
		{
			m_Curve = NewCurve();

			if(m_ShowDebug)
				m_CurveDebug = DebugDrawing.DrawCurve(transform, m_Curve.GetPoints(), "Debug_Curve", segments, m_Properties.continueStraight, m_ShowRulers);

			if(m_Properties.collisions)
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
			else if(!m_Properties.collisions || !m_ShowCollision)
			{
				if(m_HitPointDebug)
					DestroyImmediate(m_HitPointDebug);
			}
		}
	}
}
