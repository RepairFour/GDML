using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonFlourish : MonoBehaviour
{
    public float range;
    public int bladeNumber;

    public float arcAngle;
    public LayerMask hitMask;
    float intialAngle;
    public float currentAngle;
    

    public Vector3 currentBladeDirection;

    bool queued;
    bool needSetUp; 
    
    List<EnemyStats> linkedEnemies = new List<EnemyStats>();

    void Start()
    {
        intialAngle = -1* (arcAngle / 2);
        currentAngle = intialAngle;
        currentBladeDirection = Quaternion.AngleAxis(intialAngle, Vector3.up) * transform.forward;
        queued = false;
        needSetUp = true;
        
    }

    void QueueCrimsonFlourish()
    {
        if (InputManager.instance.input.Player.Ability.triggered)
        {
            queued = true;
            needSetUp = true;
            currentBladeDirection = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
        }
    }
    void SetUpCrimsonFlourish()
    {
        if(queued && needSetUp)
        {
            currentBladeDirection = Quaternion.AngleAxis(intialAngle, Vector3.up) * transform.forward;
            currentAngle = intialAngle;
            needSetUp = false;
        }
    }
    
    void ActivateCrimsonFlourish()
    {
        if (queued)
        {
            //Calculate the angle split
            var angleSplit = arcAngle / bladeNumber;
            //arcAngle/bladeNumber

            for (int i = 0; i < bladeNumber; i++)
            {
                RaycastHit[] raycastHits;
                raycastHits = Physics.SphereCastAll(transform.position, 10, transform.forward + currentBladeDirection, range, hitMask);
                currentAngle += angleSplit;

                currentBladeDirection = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
                for(int y = 0; y < raycastHits.Length; y++)
                {
                    if (raycastHits[y].transform.gameObject.CompareTag("Enemy"))
                    {
                        linkedEnemies.Add(raycastHits[i].transform.gameObject.GetComponent<EnemyStats>());
                    }
                }
            }
            queued = false;
            currentAngle = 0;

        }
        
        

    }

    void HandleCrimsonFlourishHitCooldown()
    {
        //Will need a timer to handle cooldown of the ability 

        //Once the timer has reached its limit, remove enemies from list 

        //Put the ability on cooldown
    }

    void HandleAbilityCooldown()
    {
        //Once list is empty put the ability on cooldown
    }

    // Update is called once per frame
    void Update()
    {
        QueueCrimsonFlourish();
        SetUpCrimsonFlourish();
        ActivateCrimsonFlourish();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward + currentBladeDirection * 200);
        
    }


}
