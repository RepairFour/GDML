using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    public enum Marks { NONE, BLINK};
    [SerializeField]Marks currentMark;

    public float blinkMarkDuration = 5f;
    float blinkMarkTimer = 0;
    
    public SpriteRenderer markDisplay;

    public Sprite blinkIcon;
    public void SetMark(Marks mark)
    {
        currentMark = mark;

    }

    private void Start()
    {
        SetMark(Marks.NONE);
    }
    private void Update()
    {
        switch (currentMark)
        {
            case Marks.NONE:
                break;
            case Marks.BLINK:
                blinkMarkTimer += Time.deltaTime;
                if (blinkMarkTimer >= blinkMarkDuration)
                {
                    blinkMarkApplied();
                }
                break;
        }
    }
    private void OnCollisionEnter(Collision collidedObject)
    {

        //Checks if the BlinkBullet makes connection with the object. This only works if it is not currently marked, to prevent multiple marks from being applied to the same target.
        if (collidedObject.gameObject.tag == "BlinkBullet" && currentMark != Marks.BLINK)
        {
            //Gets the sprite renderer above the object's head and sets it to the blink icon, and also flags that it is currently marked as the blink target.
            markDisplay.GetComponent<SpriteRenderer>().sprite = blinkIcon;
            SetMark(Marks.BLINK);
            GameManager.instance.playerController.SetMark(this);

        }
    }


    //This function resets the sprite display and removes the flag telling the game that the mark is above the enemy's head.
    public void blinkMarkApplied()
    {
        SetMark(Marks.NONE);
        GameManager.instance.playerController.SetMark(null);
        markDisplay.GetComponent<SpriteRenderer>().sprite = null;
        blinkMarkTimer = 0;
    }

}