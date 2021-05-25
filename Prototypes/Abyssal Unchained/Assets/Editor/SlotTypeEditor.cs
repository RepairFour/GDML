using UnityEngine;
using UnityEditor;
using Boo.Lang;
using System;
using UnityEditor.Graphs;

[CustomEditor(typeof(Slot))]
public class SlotTypeEditor : Editor
{
	SerializedObject slot;
	SerializedProperty itemsAccepted;
	SerializedProperty index;
	static string[] options;

	public override void OnInspectorGUI()
	{
		slot.Update();
		base.OnInspectorGUI();
		int x = index.intValue;

		try {
			ItemTypes itemList = FindObjectOfType<ItemTypes>();
			List<string> typesList = new List<string>(itemList.itemTypes);

			//populate options array
			options = new string[typesList.Count + 1];
			options[0] = "All";
			for (int i = 0; i < typesList.Count; ++i)
			{
				options[i + 1] = typesList[i];
			}
			//output to editor
			index.intValue = EditorGUILayout.Popup("Items Accepted", x, options);
			itemsAccepted.stringValue = options[index.intValue];

			slot.ApplyModifiedProperties();
		}
		catch(NullReferenceException exception)
		{
			Debug.LogError("Add DragDropManager to scene");
		}
	}

	private void OnEnable()
	{
		slot = new SerializedObject(target);
		index = slot.FindProperty("editorIndex");
		itemsAccepted = slot.FindProperty("itemsAccepted");
	}
}

