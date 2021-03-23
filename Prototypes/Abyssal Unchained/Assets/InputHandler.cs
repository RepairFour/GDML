using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public CharacterController2D cc;
    public AttackHandler ah;

    // Update is called once per frame
    void Update()
    {

        float movement = Input.GetAxis("Horizontal");
        cc.Move(movement);
        if (Input.GetButtonDown("Jump"))
        {
            cc.Jump();
        }

        //0 for MeleeAttack, 1 for RangedAttack
        if (Input.GetButtonDown("MeleeAttack"))
        {
            ah.HandleAttack(0);
        }
        if (Input.GetButtonDown("RangedAttack"))
        {
            ah.HandleAttack(1);
        }

    }
}
