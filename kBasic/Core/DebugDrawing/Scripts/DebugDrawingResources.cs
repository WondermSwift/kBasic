using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kBasic.Core
{
	[CreateAssetMenu(fileName = "DebugDrawingResources", menuName = "Kink Basic/Debug Drawing Resources", order = 2)]
	public class DebugDrawingResources : ScriptableObject 
	{
		public Material primaryMaterial;
		public Material secondaryMaterial;
		public float primaryWidth = 0.05f;
		public float secondaryWidth = 0.01f;
	}
}
