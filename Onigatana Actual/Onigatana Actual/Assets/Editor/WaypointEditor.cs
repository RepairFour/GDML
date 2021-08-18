using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointManager))]
public class WaypointEditor : Editor
{
	public override void OnInspectorGUI()
	{
		WaypointManager manager = (WaypointManager)target;
		base.OnInspectorGUI();
		manager.pathIdx = EditorGUILayout.Popup(manager.pathIdx, manager.pathNames.ToArray());

		if(GUILayout.Button("Add Waypoint"))
		{
			manager.BuildWaypoint(manager.pathIdx);			
		}

		EditorUtility.SetDirty(target);
	}
}
