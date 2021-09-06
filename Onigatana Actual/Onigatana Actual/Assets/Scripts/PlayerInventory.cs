using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    List<Key> keysObtained = new List<Key>();
   
    public void AddKey(Key ob)
	{
		keysObtained.Add(ob);
	}
	public void RemoveKey(Key ob)
	{
		keysObtained.Remove(ob);
	}

	public bool HasKey(int id)
	{
		foreach(var key in keysObtained)
		{
			if (key.GetId() == id)
			{
				return true;
			}
		}
		return false;
	}
}
