using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kBasic.Core;

namespace kBasic.Maths
{
	/// <summary>
	/// Amount of points used when calculating the curve.
	/// </summary>
	[ExecuteInEditMode]
	public class Curve : MonoBehaviour 
	{
		public enum CurveType
		{
			Sine,
			Cosine
		}

		// ----------------------------------------
		// Properties
		
		// Curve Parameters
		[SerializeField] private CurveType m_CurveType = CurveType.Sine;
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

		// ----------------------------------------
		// Public API

		/// <summary>
        /// Type of mathmatical curve to use.
		/// </summary>
		public CurveType curveType 
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
        /// Array of points that make up the curve.
		/// </summary>
		public Vector3[] points { get { return m_Points; }}

		/// <summary>
        /// Get an Animation Curve that represents the calculated curve.
		/// </summary>
		/// <returns> Returns an Animation Curve with values equaling the calculated curve. </returns>
		public AnimationCurve GetCurve()
		{
			AnimationCurve curve = new AnimationCurve();
			Keyframe[] curveKeys = new Keyframe[m_Points.Length];
			for(int i = 0; i < m_Points.Length; i++)
			{
				curveKeys[i] = new Keyframe((1.0f/(float)m_Points.Length) * i, m_Points[i].y); 
			}
			curve.keys = curveKeys;
			return curve;
		}

		/// <summary>
        /// Get collisions information from the curve.
		/// </summary>
		/// <param name="hitInfo"> Contains raycast hit information if active collision. </param>
		/// <returns> Returns true if the curve has an active collision. </returns>
		public bool GetCollision(out RaycastHit hitInfo)
		{
			hitInfo = m_HitInfo;
			return m_Hit;
		}

		// ----------------------------------------
		// Members

		private Vector3[] m_Points;
		private bool m_Hit;
		private Vector3 m_HitPoint;
		private RaycastHit m_HitInfo;
		private GameObject m_CurveDebug;
		private GameObject m_HitPointDebug;

		// ----------------------------------------
		// Core Methods

		private void CalculateCurve()
		{
			int segments = Mathf.Max(1, m_Segments);
			int pointCount = Mathf.Max(1, m_PointCount);

			if(m_ContinueStraight)
				m_Points = new Vector3[pointCount + 1];
			else
				m_Points = new Vector3[pointCount];
			
			for(int i = 0; i < pointCount; i++)
			{
				float time = (segments/((float)pointCount - segments)) * i;

				float pointHeight;
				switch(m_CurveType)
				{
					case CurveType.Cosine:
						pointHeight = m_Height * Mathf.Cos((time + m_Offset) * Mathf.PI);
						break;
					default:
						pointHeight = m_Height * Mathf.Sin((time + m_Offset) * Mathf.PI);
						break;
				}

				float pointLength = time * m_Distance / segments;
				m_Points[i] = new Vector3(0, pointHeight, pointLength);
			}

			if(m_ContinueStraight)
			{
				Vector3 direction = Vector3.Normalize(m_Points[pointCount - 1] - m_Points[pointCount - 2]);
				m_Points[m_Points.Length - 1] = m_Points[pointCount - 1] + direction * m_ContinueDistance;
			}

			if(m_ShowDebug)
				m_CurveDebug = DebugDrawing.DrawCurve(transform, m_Points, "Debug_Curve", segments, m_ContinueStraight, m_ShowRulers);
		}

		private void CheckCollision()
		{
			int pointCount = Mathf.Max(1, m_PointCount);

			for(int i = 0; i < pointCount - 1; i++)
			{
				Vector3 direction = m_Points[i + 1] - m_Points[i];
				float distance = Vector3.Distance(m_Points[i], m_Points[i + 1]);

				Ray ray = new Ray(m_Points[i], direction);
				RaycastHit hitInfo;
				if(Physics.Raycast(ray, out hitInfo, distance, m_CollisionLayers))
				{
					m_HitPoint = hitInfo.point;
					m_HitInfo = hitInfo;
					m_Hit = true;
					if(m_ShowDebug)
						m_HitPointDebug = DebugDrawing.DrawPoint(transform, m_HitPoint, 0.1f, "Debug_HitPoint");
					return;
				}
			}

			m_Hit = false;
			if(m_HitPointDebug)
				DestroyImmediate(m_HitPointDebug);
		}

		// ----------------------------------------
		// Timed Methods

		private void Update()
		{
			CalculateCurve();

			if(m_Collisions)
				CheckCollision();
			
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
			else if(!m_Collisions)
			{
				if(m_HitPointDebug)
					DestroyImmediate(m_HitPointDebug);
			}
		}
	}
}
