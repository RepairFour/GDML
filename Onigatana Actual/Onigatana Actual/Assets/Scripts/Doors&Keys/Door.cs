using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Door : MonoBehaviour
{
    [SerializeField] Key keyToOpen;
    bool isOpen = false;
    [SerializeField] [Range(0, 1)] float doorOpenSpeed;
    [SerializeField] bool findKey;

    private void OnTriggerEnter(Collider other)
	{
        var inv = other.GetComponent<PlayerInventory>();
        if(inv != null)
		{
            if(inv.HasKey(keyToOpen))
			{
                isOpen = true;
            }
		}
	}

	private void Update()
	{
        if(findKey)
		{
            Debug.DrawLine(transform.position, keyToOpen.transform.position);
		}
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 100, transform.position.z), doorOpenSpeed * Time.deltaTime);
        }
    }
}
