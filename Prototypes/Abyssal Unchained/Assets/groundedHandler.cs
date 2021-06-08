using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundedHandler : MonoBehaviour
{
    [SerializeField] Zenith bs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            bs.getRb.velocity = new Vector3(0, 0, 0);
            bs.isGrounded = true;
            Debug.Log("grounded");
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        bs.isGrounded = false;
    }
}
