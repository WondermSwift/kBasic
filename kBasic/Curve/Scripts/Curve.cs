using System;
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
		
		/// <summary>
		/// A class for generating sine and cosine curves. Can also check collisions along the curve.
		/// </summary>
		public Curve()
		{
			m_Points = CalculateCurve();
		}

		/// <summary>
        /// A class for generating sine and cosine curves. Can also check collisions along the curve.
		/// </summary>
		/// <param name="type"> Type of mathmatical curve to use. Sine or Cosine. </param>
		/// <param name="pointCount"> Amount of points used when calculating the curve. </param>
		/// <param name="height"> Maximum height of the curve. </param>
		/// <param name="distance"> Maximum distance of the curve. </param>
		/// <param name="segments"> Amount of curve segments to complete within the distance. </param>
		public Curve(CurveType type, int pointCount, float height, float distance, int segments = 1)
		{
			m_Properties = new Properties();
			m_Properties.curveType = type;
			m_Properties.pointCount = pointCount;
			m_Properties.height = height;
			m_Properties.distance = distance;
			m_Properties.segments = segments;
			m_Points = CalculateCurve();
		}

		/// <summary>
        /// A class for generating sine and cosine curves. Can also check collisions along the curve.
		/// </summary>
		/// <param name="type"> Type of mathmatical curve to use. Sine or Cosine. </param>
		/// <param name="pointCount"> Amount of points used when calculating the curve. </param>
		/// <param name="height"> Maximum height of the curve. </param>
		/// <param name="distance"> Maximum distance of the curve. </param>
		/// <param name="offset"> Offsets the starting point of the curve calculation. </param>
		/// <param name="segments"> Amount of curve segments to complete within the distance. </param>
		public Curve(CurveType type, int pointCount, float height, float distance, float offset, int segments = 1)
		{
			m_Properties = new Properties();
			m_Properties.curveType = type;
			m_Properties.pointCount = pointCount;
			m_Properties.height = height;
			m_Properties.distance = distance;
			m_Properties.offset = offset;
			m_Properties.segments = segments;
			m_Points = CalculateCurve();
		}

		/// <summary>
        /// A class for generating sine and cosine curves. Can also check collisions along the curve.
		/// </summary>
		/// <param name="type"> Type of mathmatical curve to use. Sine or Cosine. </param>
		/// <param name="pointCount"> Amount of points used when calculating the curve. </param>
		/// <param name="height"> Maximum height of the curve. </param>
		/// <param name="distance"> Maximum distance of the curve. </param>
		/// <param name="offset"> Offsets the starting point of the curve calculation. </param>
		/// <param name="continueStraight"> If enabled the curve will continue in a straight line after the maximum curve distance. </param>
		/// <param name="continueDistance"> Distance the line should continue after the maximum curve distance. </param>
		/// <param name="segments"> Amount of curve segments to complete within the distance. </param>
		public Curve(CurveType type, int pointCount, float height, float distance, float offset, bool continueStraight, float continueDistance, int segments = 1)
		{
			m_Properties = new Properties();
			m_Properties.curveType = type;
			m_Properties.pointCount = pointCount;
			m_Properties.height = height;
			m_Properties.distance = distance;
			m_Properties.offset = offset;
			m_Properties.continueStraight = continueStraight;
			m_Properties.continueDistance = continueDistance;
			m_Properties.segments = segments;
			m_Points = CalculateCurve();
		}

		/// <summary>
        /// A class for generating sine and cosine curves. Can also check collisions along the curve.
		/// </summary>
		/// <param name="properties"> Properties for the curve. </param>
		public Curve(Properties properties)
		{
			m_Properties = properties;
			m_Points = CalculateCurve();
		}

		// ----------------------------------------
		// Properties

		[Serializable]
		public class Properties
		{
			[SerializeField] public CurveType curveType;
			public int pointCount;
			public float height;
			public float distance;
			public float offset;
			public int segments;
			public bool continueStraight;
			public float continueDistance;
			public bool collisions;
			public LayerMask collisionLayers;

			public Properties()
			{
				curveType = CurveType.Sine;
				pointCount = 64;
				height = 1.0f;
				distance = 1.0f;
				offset = 0.0f;
				segments = 1;
				continueStraight = false;
				continueDistance = 1.0f;
				collisions = false;
				collisionLayers = 0;
			}
		}
		
		private Properties m_Properties;

		// ----------------------------------------
		// Public API

		/// <summary>
        /// Type of mathmatical curve to use.
		/// </summary>
		public CurveType curveType 
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
			m_Properties.collisions = enabled;
			m_Properties.collisionLayers = layerMask;
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
			m_Points = new Vector3[m_Properties.pointCount];
			int segments = Mathf.Max(1, m_Properties.segments);
			int pointCount = Mathf.Max(1, m_Properties.pointCount);

			if(m_Properties.continueStraight)
				m_Points = new Vector3[pointCount + 1];
			else
				m_Points = new Vector3[pointCount];
			
			for(int i = 0; i < pointCount; i++)
			{
				float time = (segments/((float)pointCount - segments)) * i;
				float pointHeight;

				switch(m_Properties.curveType)
				{
					case CurveType.Cosine:
						pointHeight = m_Properties.height * Mathf.Cos((time + m_Properties.offset) * Mathf.PI);
						break;
					default:
						pointHeight = m_Properties.height * Mathf.Sin((time + m_Properties.offset) * Mathf.PI);
						break;
				}

				float pointLength = time * m_Properties.distance / segments;
				m_Points[i] = new Vector3(0, pointHeight, pointLength);
			}

			if(m_Properties.continueStraight)
			{
				Vector3 direction = Vector3.Normalize(m_Points[pointCount - 1] - m_Points[pointCount - 2]);
				m_Points[pointCount] = m_Points[pointCount - 1] + direction * m_Properties.continueDistance;
			}
			
			return m_Points;
		}

		private bool CalculateCollision(Vector3[] points, out RaycastHit hitInfo)
		{
			int pointCount = Mathf.Max(1, m_Properties.pointCount);

			for(int i = 0; i < pointCount - 1; i++)
			{
				Vector3 direction = points[i + 1] - points[i];
				float distance = Vector3.Distance(points[i], points[i + 1]);
				Ray ray = new Ray(points[i], direction);
				RaycastHit hit;
				
				if(Physics.Raycast(ray, out hit, distance, m_Properties.collisionLayers))
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
