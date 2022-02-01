using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonFlourish : MonoBehaviour
{
    public float range;
    public int bladeNumber;
    public float arcAngle;
    public LayerMask hitMask;
    public float activationTime;
    public float cooldownTime;
    [Range(0.25f, 1)] public float damageModifer; 

    public GameObject tempThing;
    
    float intialAngle;
    float currentAngle;
    float cooldownTimer;
    float activationTimer;
    Vector3 currentBladeDirection;

    bool queued;
    bool needSetUp;
    
    bool onCooldown;
    bool abilityActivated;

    public LineRenderer lr;
    List<EnemyStats> linkedEnemies = new List<EnemyStats>();

    void Start()
    {
        intialAngle = -1* (arcAngle / 2);
        currentAngle = intialAngle;
        currentBladeDirection = Quaternion.AngleAxis(intialAngle, Vector3.up) * transform.forward;
        queued = false;
        needSetUp = true;
        
    }
    
    public ref List<EnemyStats> getLinkedEnemies()
    {
        return ref linkedEnemies;
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
        if (queued && !abilityActivated)
        {
            //Calculate the angle split
            var angleSplit = arcAngle / bladeNumber;
            //arcAngle/bladeNumber

            for (int i = 0; i < bladeNumber; i++)
            {
                RaycastHit[] raycastHits;
                raycastHits = Physics.SphereCastAll(transform.position, 10, transform.forward + currentBladeDirection, range, hitMask);
                currentAngle += angleSplit;

                var temp = Instantiate(tempThing, transform.position + transform.forward * 2, Quaternion.identity); 
                temp.GetComponent<Rigidbody>().AddForce(transform.forward + currentBladeDirection * 10000);

                currentBladeDirection = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
                for(int y = 0; y < raycastHits.Length; y++)
                {
                    if (raycastHits[y].transform.gameObject.CompareTag("Enemy"))
                    {
                        if (!linkedEnemies.Contains(raycastHits[y].transform.gameObject.GetComponent<EnemyStats>()))
                        { 
                            linkedEnemies.Add(raycastHits[y].transform.gameObject.GetComponent<EnemyStats>());
                        }
                    }
                }
            }
            //queued = false;
            abilityActivated = true;
            currentAngle = 0;
            //enemiesAddedToList = true;

        }
        
        

    }

    void HandleCrimsonFlourishHitCooldown()
    {
        if(linkedEnemies.Count > 0)
        {
            activationTimer += Time.deltaTime;
            if(activationTimer >= activationTime)
            {
                linkedEnemies.Clear();
                activationTimer = 0;
                onCooldown = true;
            }
        }
    }

    void HandleAbilityCooldown()
    {
        if (onCooldown || (linkedEnemies.Count == 0 && abilityActivated))
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                onCooldown = false;
                abilityActivated = false;
                queued = false;
                cooldownTimer = 0;
            }
        }
    }

    void DrawLinks()
    {
        if(linkedEnemies.Count > 0)
        {
            lr.positionCount = linkedEnemies.Count + 1;

            if (linkedEnemies[0] != null)
            {
                lr.SetPosition(0, linkedEnemies[0].gameObject.transform.position);
            }
            for(int i = 0; i < linkedEnemies.Count; i++)
            {
                if (i != linkedEnemies.Count - 1)
                {
                    if (linkedEnemies[i + 1] != null)
                    {
                        lr.SetPosition(i + 1, linkedEnemies[i + 1].gameObject.transform.position);
                    }
                }
                else
                {
                    if (linkedEnemies[0] != null)
                    {
                        lr.SetPosition(i + 1, linkedEnemies[0].gameObject.transform.position);
                    }
                    
                }
            }
        }
        if(linkedEnemies.Count == 0)
        {
            lr.positionCount = 0;
        }
    }

    void CleanList()
    {
        for (int i = linkedEnemies.Count - 1; i >= 0; i--)
        {
            if(linkedEnemies[i] == null)
            {
                linkedEnemies.Remove(linkedEnemies[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        QueueCrimsonFlourish();
        SetUpCrimsonFlourish();
        ActivateCrimsonFlourish();
        HandleCrimsonFlourishHitCooldown();
        HandleAbilityCooldown();
        DrawLinks();
        CleanList();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward + currentBladeDirection * 200);
        
    }


}
