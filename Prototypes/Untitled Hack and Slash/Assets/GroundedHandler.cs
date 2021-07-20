using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedHandler : MonoBehaviour
{
    [SerializeField]bool Grounded = true;

    public bool isGrounded { get => Grounded; set => Grounded = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Grounded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
        }
    }
}
