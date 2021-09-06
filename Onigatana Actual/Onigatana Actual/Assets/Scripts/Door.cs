using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Key keyToOpen;
    [SerializeField] int id;
    bool isOpen = false;
    [SerializeField][Range(0,1)] float doorOpenSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        keyToOpen.SetId(id);
    }

	private void OnTriggerEnter(Collider other)
	{
        var inv = other.GetComponent<PlayerInventory>();
        if(inv != null)
		{
            if(inv.HasKey(id))
			{
                isOpen = true;
			}
		}
	}

	private void Update()
	{
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 100, transform.position.z), doorOpenSpeed * Time.deltaTime);
        }
    }
}
