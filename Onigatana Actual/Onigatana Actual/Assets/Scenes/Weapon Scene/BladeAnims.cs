using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class BladeAnims : MonoBehaviour
{
    PlayerMap Input;
    private ButtonControl meleeButton;

    public Animator meleeAnims;

    private bool isInAttack1Anim;
    private bool isInAttack2Anim;

    private float timeCharged;
    public float incrementsPerSecondCharged;
    public float timeRequiredForChargedAttack;

    public MeshRenderer RangedWeapon;
    public GunTestTeleport guntestscript;
    public MeshRenderer meleeWeapon;

    public GameObject AttacksHitbox1;




    private void Start()
    {
        Input = new PlayerMap();
        Input.Enable();

        meleeButton = (ButtonControl)Input.Player.Melee.controls[0];

        //We use these variables to tell which state the weapon is in.
        isInAttack1Anim = false;
        isInAttack2Anim = false;

        AttacksHitbox1.SetActive(false);

        meleeWeapon.enabled = false;
        guntestscript.enabled = true;
        RangedWeapon.enabled = true;
    }



    private void Update()
    {

        //We use this function 
        if (meleeButton.isPressed)
        {
            meleeWeapon.enabled = true;
            RangedWeapon.enabled = false;
            guntestscript.enabled = false;

            timeCharged += incrementsPerSecondCharged * Time.deltaTime;
        }

        if (meleeButton.wasReleasedThisFrame) 
        {


            Debug.Log(timeCharged);

            //Handles the first attack. It cancels the noLongerAttacking invoke if the Invoke is currently being called, to prevent animation overlap. It then Invokes the function again, to begin counting it down.
            //We check the TimeCharged and TimeRequiredForChargedAttack variables, in order to see whether it should be executing the charged attack instead. We also check if the attack sequence is in a state ready to perform this attack.
            if (timeCharged < timeRequiredForChargedAttack && isInAttack1Anim == false)
            {
                timeCharged = 0;
                RangedWeapon.enabled = false;
                guntestscript.enabled = false;
                meleeWeapon.enabled = true;

                AttacksHitbox1.SetActive(true);

                meleeAnims.SetTrigger("Attack1");
                isInAttack1Anim = true;
                isInAttack2Anim = false;
                CancelInvoke("noLongerAttacking");
                Invoke("noLongerAttacking", 0.5f);
            }


            //Importantly, this is an Else If, making sure that this is always checked AFTER the first statement.
            //Functions almost identically to the first If statement, however it only plays if Anim1 was activated, preventing this from being played directly after noLongerAttacking is called.
            else if (timeCharged < timeRequiredForChargedAttack && isInAttack1Anim == true)
            {
                timeCharged = 0;
                RangedWeapon.enabled = false;
                guntestscript.enabled = false;
                meleeWeapon.enabled = true;

                AttacksHitbox1.SetActive(true);

                meleeAnims.SetTrigger("Attack2");
                isInAttack1Anim = false;
                isInAttack2Anim = true;
                CancelInvoke("noLongerAttacking");
                Invoke("noLongerAttacking", 0.5f);
            }

            else if (timeCharged >= timeRequiredForChargedAttack && isInAttack1Anim == false && isInAttack2Anim == false)
            {
                timeCharged = 0;
                RangedWeapon.enabled = false;
                guntestscript.enabled = false;
                meleeWeapon.enabled = true;
                meleeAnims.SetTrigger("ChargedAttack");
                isInAttack1Anim = false;
                isInAttack2Anim = false;
                CancelInvoke("NoLongerAttacking");
                Invoke("noLongerAttacking", 0.5f);

            }

            Invoke("disableHitbox", 0.1f);
            timeCharged = 0;

        }
       








    }




    //Resets the anim back to the idle state, clearing the variables back to false.
    private void noLongerAttacking() 
    {
        isInAttack1Anim = false;
        isInAttack2Anim = false;
        meleeAnims.SetTrigger("reset");

        RangedWeapon.enabled = true;
        guntestscript.enabled = true;
        meleeWeapon.enabled = false;
        
    }

    private void disableHitbox()
    {
        Debug.Log("CancelledHitbox");
        AttacksHitbox1.SetActive(false);
    }

}

