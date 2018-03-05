using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using kBasic.Core;

namespace kBasic.Maths
{
	[CustomEditor(typeof(Curve))]
	public class CurveEditor : Editor 
	{
		internal class Styles
        {
			public static GUIContent curveTypeLabel = new GUIContent("Curve Type", "Type of mathmatical curve to use.");
            public static GUIContent pointCountLabel = new GUIContent("Point Count", "Amount of points used when calculating the curve.");
			public static GUIContent heightLabel = new GUIContent("Height", "Maximum height of the curve.");
			public static GUIContent distanceLabel = new GUIContent("Distance", "Maximum distance of the curve.");
			public static GUIContent offsetLabel = new GUIContent("Offset", "Offsets the starting point of the curve calculation.");
			public static GUIContent segmentsLabel = new GUIContent("Segments", "Amount of curve segments to complete within the distance.");
			public static GUIContent continueStraightLabel = new GUIContent("Continue Straight", "If enabled the curve will continue in a straight line after the maximum curve distance.");
			public static GUIContent continueDistanceLabel = new GUIContent("Distance", "Distance the line should continue after the maximum curve distance.");
			public static GUIContent collisionsLabel = new GUIContent("Enable Collisions", "Enables collisions for the curve. Calculates a world space point at the first collision.");
			public static GUIContent collisionLayersLabel = new GUIContent("Layers", "Defines which layers the curve should collide with.");
			public static GUIContent showDebugLabel = new GUIContent("Show Debug", "Enables debug drawing in player and edit modes.");
			public static GUIContent showRulersLabel = new GUIContent("Rulers", "Enables debug rulers for maximum curve distance and height.");

			public static string[] curveTypeOptions = {"Sine", "Cosine"};
		}

		private SerializedProperty m_CurveType;
		private SerializedProperty m_PointCount;
		private SerializedProperty m_Height;
		private SerializedProperty m_Distance;
		private SerializedProperty m_Offset;
		private SerializedProperty m_Segments;
		private SerializedProperty m_ContinueStraight;
		private SerializedProperty m_ContinueDistance;
		private SerializedProperty m_Collisions;
		private SerializedProperty m_CollisionLayers;
		private SerializedProperty m_ShowDebug;
		private SerializedProperty m_ShowRulers;

		void OnEnable()
        {
			m_CurveType = serializedObject.FindProperty("m_CurveType");
            m_PointCount = serializedObject.FindProperty("m_PointCount");
			m_Height = serializedObject.FindProperty("m_Height");
			m_Distance = serializedObject.FindProperty("m_Distance");
			m_Offset = serializedObject.FindProperty("m_Offset");
			m_Segments = serializedObject.FindProperty("m_Segments");
			m_ContinueStraight = serializedObject.FindProperty("m_ContinueStraight");
			m_ContinueDistance = serializedObject.FindProperty("m_ContinueDistance");
			m_Collisions = serializedObject.FindProperty("m_Collisions");
			m_CollisionLayers = serializedObject.FindProperty("m_CollisionLayers");
			m_ShowDebug = serializedObject.FindProperty("m_ShowDebug");
			m_ShowRulers = serializedObject.FindProperty("m_ShowRulers");
        }

		protected void DoPopup(GUIContent label, SerializedProperty property, string[] options)
        {
            var mode = property.intValue;
            EditorGUI.BeginChangeCheck();

            if (mode >= options.Length)
                Debug.LogError(string.Format("Invalid option while trying to set {0}", label.text));

            mode = EditorGUILayout.Popup(label, mode, options);
            if (EditorGUI.EndChangeCheck())
                property.intValue = mode;
        }

		public override void OnInspectorGUI()
        {
			serializedObject.Update();

            EditorGUILayout.Space();

			EditorGUILayout.LabelField("Curve Parameters", EditorStyles.boldLabel);
			DoPopup(Styles.curveTypeLabel, m_CurveType, Styles.curveTypeOptions);
			EditorGUILayout.PropertyField(m_PointCount, Styles.pointCountLabel);
			EditorGUILayout.PropertyField(m_Height, Styles.heightLabel);
			EditorGUILayout.PropertyField(m_Distance, Styles.distanceLabel);
			EditorGUILayout.PropertyField(m_Offset, Styles.offsetLabel);
			EditorGUILayout.PropertyField(m_Segments, Styles.segmentsLabel);

			EditorGUILayout.PropertyField(m_ContinueStraight, Styles.continueStraightLabel);
			if(m_ContinueStraight.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_ContinueDistance, Styles.continueDistanceLabel);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(m_Collisions, Styles.collisionsLabel);
			if(m_Collisions.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_CollisionLayers, Styles.collisionLayersLabel);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(m_ShowDebug, Styles.showDebugLabel);
			if(m_ShowDebug.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_ShowRulers, Styles.showRulersLabel);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }
	}
}
