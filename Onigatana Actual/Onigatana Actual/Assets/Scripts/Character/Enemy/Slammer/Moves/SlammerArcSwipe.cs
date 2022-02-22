using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerArcSwipe : SlammerMove
{
    [SerializeField] float arcAngle;
    [SerializeField] float arcSpeed;
    [Tooltip("The speed at the very start that gets the target facing the player")]
    [SerializeField] float adjustmentSpeed;
    bool inPosition = false;
    Vector3 playerPosOnFrame;
    bool firstFrame = false;
    GameObject player;

    private void Start()
	{
        player = FindObjectOfType<PlayerStats>().gameObject;
    }
	public override bool Activate()
	{
        //setup vars
        float halfArc = arcAngle / 2;
        Vector3 position = new Vector3(playerPosOnFrame.x, transform.position.y, playerPosOnFrame.z);
        var dir = position - transform.position;
        dir.Normalize();
        var rotGoal = Quaternion.LookRotation(dir);

        if (!inPosition)
        {
            if (!firstFrame)
            {
                firstFrame = true;
                playerPosOnFrame = player.transform.position;
            }
            //rotation to start pos
            
            rotGoal.eulerAngles = new Vector3(rotGoal.eulerAngles.x, rotGoal.eulerAngles.y - halfArc, rotGoal.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, adjustmentSpeed * Time.deltaTime);
            if (Mathf.Abs((transform.rotation.eulerAngles - rotGoal.eulerAngles).magnitude) < 0.1f)
            {
                inPosition = true;
            }
        }
        else
        {
            rotGoal.eulerAngles = new Vector3(rotGoal.eulerAngles.x, rotGoal.eulerAngles.y + halfArc, rotGoal.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, arcSpeed * Time.deltaTime);
            //end = reset vars
            if (Mathf.Abs((transform.rotation.eulerAngles - rotGoal.eulerAngles).magnitude) < 0.1f)
            {
                inPosition = false;
                return false;
                firstFrame = false;
            }
        }
        return true;
    }
}
