using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kBasic.Core;

namespace kBasic.Maths
{
	/// <summary>
	/// A class for generating sine and cosine curves. Can also check collisions along the curve.
	/// </summary>
	[ExecuteInEditMode]
	public class Curve 
	{
		// ----------------------------------------
		// Enums

		public enum CurveType
		{
			Sine,
			Cosine
		}

		// ----------------------------------------
		// Constructors

		public Curve()
		{
			m_Points = CalculateCurve();
		}

		public Curve(CurveType type, int pointCount, float height, float distance, int segments = 1)
		{
			m_CurveType = type;
			m_PointCount = pointCount;
			m_Height = height;
			m_Distance = distance;
			m_Segments = segments;
			m_Points = CalculateCurve();
		}

		public Curve(CurveType type, int pointCount, float height, float distance, float offset, int segments = 1)
		{
			m_CurveType = type;
			m_PointCount = pointCount;
			m_Height = height;
			m_Distance = distance;
			m_Offset = offset;
			m_Segments = segments;
			m_Points = CalculateCurve();
		}

		public Curve(CurveType type, int pointCount, float height, float distance, float offset, bool continueStraight, float continueDistance, int segments = 1)
		{
			m_CurveType = type;
			m_PointCount = pointCount;
			m_Height = height;
			m_Distance = distance;
			m_Offset = offset;
			m_ContinueStraight = continueStraight;
			m_ContinueDistance = continueDistance;
			m_Segments = segments;
			m_Points = CalculateCurve();
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
        /// Get an Animation Curve that represents the calculated curve.
		/// </summary>
		/// <returns> Returns an Animation Curve with values equaling the calculated curve. </returns>
		public AnimationCurve GetAnimationCurve()
		{
			if(m_Points == null)
				m_Points = CalculateCurve();
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
        /// Get the points of the calculated curve.
		/// </summary>
		/// <returns> Returns a Vector 3 array of the calculate curve points. </returns>
		public Vector3[] GetPoints()
		{
			if(m_Points == null)
				m_Points = CalculateCurve();
			return m_Points;
		}

		/// <summary>
        /// Set collision parameters the curve.
		/// </summary>
		/// <param name="enabled"> Sets collision check on or off. </param>
		/// <param name="layerMask"> Sets which layers should be used for collision. </param>
		public void SetCollisionParameters(bool enabled, int layerMask)
		{
			m_Collisions = enabled;
			m_CollisionLayers = layerMask;
		}

		/// <summary>
        /// Get collision information the curve.
		/// </summary>
		/// <param name="hitInfo"> Contains raycast hit information if active collision. </param>
		/// <returns> Returns true if the curve has an active collision. </returns>
		public bool GetCollision(out RaycastHit hitInfo)
		{
			if(m_Points == null)
				m_Points = CalculateCurve();
			return CalculateCollision(m_Points, out hitInfo);
		}

		// ----------------------------------------
		// Core Methods

		private Vector3[] m_Points;

		private Vector3[] CalculateCurve()
		{
			m_Points = new Vector3[m_PointCount];
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
				m_Points[pointCount] = m_Points[pointCount - 1] + direction * m_ContinueDistance;
			}
			
			return m_Points;
		}

		private bool CalculateCollision(Vector3[] points, out RaycastHit hitInfo)
		{
			int pointCount = Mathf.Max(1, m_PointCount);

			for(int i = 0; i < pointCount - 1; i++)
			{
				Vector3 direction = points[i + 1] - points[i];
				float distance = Vector3.Distance(points[i], points[i + 1]);
				Ray ray = new Ray(points[i], direction);
				RaycastHit hit;
				
				if(Physics.Raycast(ray, out hit, distance, m_CollisionLayers))
				{
					hitInfo = hit;
					return true;
				}
			}	

			hitInfo = new RaycastHit();
			return false;
		}
	}
}
