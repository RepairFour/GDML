using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUI : MonoBehaviour
{
    [SerializeField] List<Transform> keySlots;
    int slotsFilled = 0;
    public bool FillSlot(GameObject key)
	{
        if(slotsFilled < keySlots.Count)
		{
			key.transform.parent = keySlots[slotsFilled];
			key.transform.localPosition = Vector3.zero;
			key.transform.localRotation = Quaternion.identity;
			key.transform.localScale = new Vector3(100,100,1);
			++slotsFilled;
			return true;			
		}
        return false;
	}
}
