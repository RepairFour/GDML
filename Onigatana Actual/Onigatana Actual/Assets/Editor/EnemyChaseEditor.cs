using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyChase))]
public class EnemyChaseEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EnemyChase script = (EnemyChase)target;
		WaypointManager manager = FindObjectOfType<WaypointManager>();
		base.OnInspectorGUI();
		script.patrolPath = EditorGUILayout.Popup(script.patrolPath, manager.pathNames.ToArray());
	}

}
