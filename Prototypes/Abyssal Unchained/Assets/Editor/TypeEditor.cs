using UnityEngine;
using UnityEditor;
using Boo.Lang;
using System;

[CustomEditor(typeof(Draggable))]
public class TypeEditor : Editor
{
	#region Variables
	SerializedObject draggable;
	SerializedProperty itemType;
	SerializedProperty index;
	static string[] options;
	#endregion

	#region Methods
	public override void OnInspectorGUI()
	{
		draggable.Update();
		base.OnInspectorGUI();
		int x = index.intValue;

		try
		{
			ItemTypes itemList = FindObjectOfType<ItemTypes>();
			List<string> typesList = new List<string>(itemList.itemTypes);

			//populate options array
			options = new string[typesList.Count];
			for (int i = 0; i < typesList.Count; ++i)
			{
				options[i] = typesList[i];
			}
			//output to editor
			index.intValue = EditorGUILayout.Popup("Item Type", x, options);
			itemType.stringValue = options[index.intValue];
			draggable.ApplyModifiedProperties();
		}
		catch(NullReferenceException exception)
		{
			Debug.LogError("Add DragDropManager to scene");
		}
	}

	private void OnEnable()
	{
		draggable = new SerializedObject(target);
		index = draggable.FindProperty("editorIndex");
		itemType = draggable.FindProperty("itemType");
	}
	#endregion
}
