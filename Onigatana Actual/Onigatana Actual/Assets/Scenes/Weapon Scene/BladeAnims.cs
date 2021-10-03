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

    //This variable exists to prevent a bug in which you could glitch out the charged attack by spamming quick attacks after performing one.
    private bool canAttack = true;

    private float timeCharged;
    public float incrementsPerSecondCharged;
    public float timeRequiredForChargedAttack;

    public MeshRenderer RangedWeapon;
    public GunTestTeleport guntestscript;
    public MeshRenderer meleeWeapon;

    //There is almost certainly a better way to handle our attacks than this. Josh/Cam, please look into this.
    public GameObject AttacksHitbox1;




    private void Start()
    {
        Input = new PlayerMap();
        Input.Enable();

        meleeButton = (ButtonControl)Input.Player.Melee.controls[0];

        //We use these variables to tell which state the weapon is in.
        isInAttack1Anim = false;
        isInAttack2Anim = false;

        //I honestly hate the way this works. Its inconsistent as fuck. We HAVE to find a better way to use the melee hitbox.
        AttacksHitbox1.SetActive(false);

        //Here we set the variables so that the gun is active, but the melee weapon isnt. We will turn these values on and off in each case.
        meleeWeapon.enabled = false;
        guntestscript.enabled = true;
        RangedWeapon.enabled = true;
    }



    private void Update()
    {

        //We use this function to calculate how long the button is held down for. This will be important for distinguishing whether to perform a quick melee or a charged melee.
        if (meleeButton.isPressed)
        {
            meleeWeapon.enabled = true;
            RangedWeapon.enabled = false;
            guntestscript.enabled = false;

            timeCharged += incrementsPerSecondCharged * Time.deltaTime;
        }


        //Here we handle the result of the above function, and use timeCharged to calculate which attack we should perform, in addition to a couple other factors.
        if (meleeButton.wasReleasedThisFrame) 
        {


            Debug.Log(timeCharged);

            //Handles the first attack. It cancels the noLongerAttacking invoke if the Invoke is currently being called, to prevent animation overlap. It then Invokes the function again, to begin counting it down.
            //We check the TimeCharged and TimeRequiredForChargedAttack variables, in order to see whether it should be executing the charged attack instead. We also check if the attack sequence is in a state ready to perform this attack.
            if (timeCharged < timeRequiredForChargedAttack && isInAttack1Anim == false && canAttack == true)
            {
                timeCharged = 0;
                RangedWeapon.enabled = false;
                guntestscript.enabled = false;
                meleeWeapon.enabled = true;

                //We turn on the attack hitbox. This is a horrible way of handling the damage hitbox, please look into alternative solutions.
                AttacksHitbox1.SetActive(true);

                meleeAnims.SetTrigger("Attack1");
                isInAttack1Anim = true;
                isInAttack2Anim = false;
                CancelInvoke("noLongerAttacking");
                Invoke("noLongerAttacking", 0.5f);
            }


            //Importantly, this is an Else If, making sure that this is always checked AFTER the first statement.
            //Functions almost identically to the first If statement, however it only plays if Anim1 was activated, preventing this from being played directly after noLongerAttacking is called.
            //We can use this to set a different animation for the followup attack, giving a nice back and forth to the animations.
            else if (timeCharged < timeRequiredForChargedAttack && isInAttack1Anim == true && canAttack == true)
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

            //Handles the charged attack.
            else if (timeCharged >= timeRequiredForChargedAttack && isInAttack1Anim == false && isInAttack2Anim == false && canAttack == true)
            {
                canAttack = false;
                timeCharged = 0;
                RangedWeapon.enabled = false;
                guntestscript.enabled = false;
                meleeWeapon.enabled = true;
                meleeAnims.SetTrigger("ChargedAttack");
                isInAttack1Anim = false;
                isInAttack2Anim = false;
                CancelInvoke("NoLongerAttacking");
                Invoke("noLongerAttacking", 0.35f);

            }

            //We disable the attack hitbox very quickly. Again, this is a bandaid on a bad system for hit detection. Pl0x update it.
            Invoke("disableHitbox", 0.2f);
            timeCharged = 0;

        }
       








    }




    //Resets the anim back to the idle state, clearing the variables back to false.
    private void noLongerAttacking() 
    {
        isInAttack1Anim = false;
        isInAttack2Anim = false;
        meleeAnims.SetTrigger("reset");
        canAttack = true;

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

