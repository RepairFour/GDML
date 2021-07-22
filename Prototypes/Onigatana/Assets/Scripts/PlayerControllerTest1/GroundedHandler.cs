using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedHandler : MonoBehaviour
{
    [SerializeField]bool grounded;

    public bool isGrounded { get => grounded; set => grounded = value; }

    private void Start()
    {
        grounded = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
    
}
