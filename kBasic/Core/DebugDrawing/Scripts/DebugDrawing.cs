using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

namespace kBasic.Core
{
	public static class DebugDrawing 
	{
		private static DebugDrawingResources m_DebugResources;
		private static DebugDrawingResources debugResources
		{
			get
			{
				if(m_DebugResources == null)
					m_DebugResources = (DebugDrawingResources)Resources.Load("DebugDrawingResources");
				return m_DebugResources;
			}
		}

		public static GameObject DrawPoint(Transform root, Vector3 point, float scale, string name = "Debug_Point")
		{
			Vector3 vScale = new Vector3(scale, scale, scale);

			Transform transformPoint = root.Find(name);
			if(!transformPoint)
			{
				GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				obj.name = name;
				transformPoint = obj.transform;
				transformPoint.parent = root;
				transformPoint.localRotation = Quaternion.identity;
				transformPoint.localScale = vScale;
				obj.GetComponent<MeshRenderer>().material = debugResources.primaryMaterial;
			}
			transformPoint.position = point;
			return transformPoint.gameObject;
		}

		public static GameObject DrawCurve(Transform root, Vector3[] points, string name = "Debug_Curve", int segments = 1, bool continueLine = false, bool drawRulers = false)
		{
			// Horizontal ruler
			Transform transformCurve = root.Find(name);
			if(!transformCurve)
			{
				GameObject obj = new GameObject();
				obj.name = name;
				transformCurve = obj.transform;
				transformCurve.parent = root;
				transformCurve.localPosition = Vector3.zero;
				transformCurve.localRotation = Quaternion.identity;
			}

			LineRenderer line = transformCurve.GetComponent<LineRenderer>();
			if(!line)
				line = transformCurve.gameObject.AddComponent<LineRenderer>();

			line.positionCount = points.Length;
			line.material = debugResources.primaryMaterial;
			line.startWidth = debugResources.primaryWidth;
			line.endWidth = debugResources.primaryWidth;

			for(int i = 0; i < points.Length; i++)
			{
				line.SetPosition(i, transformCurve.TransformPoint(points[i]));
			}

			if(drawRulers)
			{
				// Parent
				Transform transformParent = transformCurve.Find("Rulers");
				if(!transformParent)
				{
					GameObject obj = new GameObject();
					obj.name = "Rulers";
					transformParent = obj.transform;
					transformParent.parent = transformCurve;
					transformParent.localPosition = Vector3.zero;
					transformParent.localRotation = Quaternion.identity;
				}

				// Get useful points
				int endPointIndex = (int)(points.Length) - segments;
				if(continueLine)
					endPointIndex -- ;
				Vector3 firstPoint = new Vector3(points[0].x, transformCurve.position.y, points[0].z);
				Vector3 arcEndPoint = new Vector3(points[endPointIndex].x, transformCurve.position.y, points[endPointIndex].z);
				float maxMeight = 0.0f;
				foreach(Vector3 v in points)
					maxMeight = Mathf.Max(maxMeight, v.y);
				Vector3 verticalPoint = new Vector3 (0, maxMeight, arcEndPoint.z);

				// Horizontal ruler
				Transform transformH = transformParent.Find("HorziontalRuler");
				if(!transformH)
				{
					GameObject obj = new GameObject();
					obj.name = "HorziontalRuler";
					transformH = obj.transform;
					transformH.parent = transformParent;
					transformH.localPosition = Vector3.zero;
					transformH.localRotation = Quaternion.identity;
				}

				LineRenderer horizontalRuler = transformH.GetComponent<LineRenderer>();
				if(!horizontalRuler)
					horizontalRuler = transformH.gameObject.AddComponent<LineRenderer>();

				horizontalRuler.positionCount = 2;
				horizontalRuler.material = debugResources.secondaryMaterial;
				horizontalRuler.startWidth = debugResources.secondaryWidth;
				horizontalRuler.endWidth = debugResources.secondaryWidth;
				horizontalRuler.SetPositions(new Vector3[] {firstPoint, arcEndPoint});

				// Vertical ruler
				Transform transformV = transformParent.Find("VerticalRuler");
				if(!transformV)
				{
					GameObject obj = new GameObject();
					obj.name = "VerticalRuler";
					transformV = obj.transform;
					transformV.parent = transformParent;
					transformV.localPosition = Vector3.zero;
					transformV.localRotation = Quaternion.identity;
				}

				LineRenderer verticalRuler = transformV.GetComponent<LineRenderer>();
				if(!verticalRuler)
					verticalRuler = transformV.gameObject.AddComponent<LineRenderer>();

				verticalRuler.positionCount = 2;
				verticalRuler.material = debugResources.secondaryMaterial;
				verticalRuler.startWidth = debugResources.secondaryWidth;
				verticalRuler.endWidth = debugResources.secondaryWidth;

				verticalRuler.SetPositions(new Vector3[] {arcEndPoint, verticalPoint});

				// Horizontal label
				Transform transformLabelH = transformParent.Find("HorziontalRulerLabel");
				TextMesh labelH;
				if(!transformLabelH)
				{
					GameObject obj = new GameObject();
					obj.name = "HorziontalRulerLabel";
					transformLabelH = obj.transform;
					transformLabelH.parent = transformParent;
					transformLabelH.localEulerAngles = new Vector3(0, -90, 0);
					transformLabelH.gameObject.AddComponent<TextMesh>();
				}
				transformLabelH.position = Vector3.Lerp(firstPoint, arcEndPoint, 0.5f);
				labelH = transformLabelH.GetComponent<TextMesh>();
				var heightValue = Vector3.Distance(firstPoint, arcEndPoint);
				labelH.text = string.Format("{0:0.00}", heightValue);
				labelH.characterSize = 0.1f;

				// Vertical label
				Transform transformLabelV = transformParent.Find("VerticalRulerLabel");
				TextMesh labelV;
				if(!transformLabelV)
				{
					GameObject obj = new GameObject();
					obj.name = "VerticalRulerLabel";
					transformLabelV = obj.transform;
					transformLabelV.parent = transformParent;
					transformLabelV.localEulerAngles = new Vector3(0, -90, 0);
					transformLabelV.gameObject.AddComponent<TextMesh>();
				}
				transformLabelV.position = Vector3.Lerp(arcEndPoint, verticalPoint, 0.5f);
				labelV = transformLabelV.GetComponent<TextMesh>();
				var distanceValue = Vector3.Distance(arcEndPoint, verticalPoint);
				labelV.text = string.Format("{0:0.00}", heightValue);
				labelV.characterSize = 0.1f;
			}
			else
			{
				Transform transformParent = transformCurve.Find("Rulers");
				if(transformParent)
					GameObject.DestroyImmediate(transformParent.gameObject);
			}

			return transformCurve.gameObject;
		}
	}
}
