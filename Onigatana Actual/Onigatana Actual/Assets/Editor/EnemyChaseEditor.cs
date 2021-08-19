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
		SerializedProperty pathNum = serializedObject.FindProperty("patrolPath");
		WaypointManager manager = FindObjectOfType<WaypointManager>();
		base.OnInspectorGUI();
		script.patrolPath = EditorGUILayout.Popup(script.patrolPath, manager.pathNames.ToArray());
		pathNum.intValue = script.patrolPath;
		serializedObject.ApplyModifiedProperties();
	}

}
