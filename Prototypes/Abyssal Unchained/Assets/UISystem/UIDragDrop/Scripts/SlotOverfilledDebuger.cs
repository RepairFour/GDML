using Boo.Lang;
using UnityEngine;

[ExecuteInEditMode]
public class SlotOverfilledDebuger : MonoBehaviour
{
	private List<Slot> slots;

	private void Update()
	{
		slots = new List<Slot>(FindObjectsOfType<Slot>());
		foreach (var slot in slots)
		{
			if (slot.GetComponentsInChildren<Draggable>().Length > 1)
			{
				Debug.LogError(slot.name + " has more that 1 item in it");
			}
		}

	}
}
