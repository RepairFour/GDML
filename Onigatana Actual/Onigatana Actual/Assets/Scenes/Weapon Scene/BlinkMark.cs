using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;

public class BlinkMark : MonoBehaviour
{
    PlayerMap Input;
    //private ButtonControl meleeButton;


    public SpriteRenderer markDisplay;

    public Sprite blinkIcon;
    
    public GameObject player;


    private bool isMarkedBlink;
    private bool buttonIsPressed;

    private void Start()
    {
        //Sets the player's Melee Key to activate the blink.
        
        Input = new PlayerMap();
        Input.Enable();
        //meleeButton = (ButtonControl)Input.Player.Melee.controls[0];

        isMarkedBlink = false;


        //We find the player in the scene.
        //player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
       if (Input.Player.Melee.triggered)
       {
            meleeAbility();
       }

    }

    private void meleeAbility()
    {
        if (isMarkedBlink == true)
        {
            Debug.Log("meleePressed");
            Debug.Log("Player Transform before " + player.transform.position);
            player.GetComponent<Controller>().TeleportToPosition(transform.position);
            Debug.Log("Player Transform after " + player.transform.position);


        }
    }

    private void OnCollisionEnter(Collision collidedObject)
    {

        //Checks if the BlinkBullet makes connection with the object. This only works if it is not currently marked, to prevent multiple marks from being applied to the same target.
        if (collidedObject.gameObject.tag == "BlinkBullet" && isMarkedBlink == false)
        {
            //Gets the sprite renderer above the object's head and sets it to the blink icon, and also flags that it is currently marked as the blink target.
            markDisplay.GetComponent<SpriteRenderer>().sprite = blinkIcon;
            isMarkedBlink = true;

            
            //player.transform.position = new Vector3(100, 110, 111);


            //Call this to begin ticking down the mark's duration.
            Invoke("blinkMarkApplied", 5.0f);
        }
    }


    //This function resets the sprite display and removes the flag telling the game that the mark is above the enemy's head.
    void blinkMarkApplied()
    {
        isMarkedBlink = false;
        markDisplay.GetComponent<SpriteRenderer>().sprite = null;
    }

}
