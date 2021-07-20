using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public LayerMask targetLayer;
    public float range = 2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, range, targetLayer))
            {
                Debug.Log("Enemy Hit");
                if (hit.collider.CompareTag("VerticalLimb"))
                {
                    hit.collider.gameObject.GetComponent<LimbStat>().TakeDamage(1);

                }
                else
                {
                    hit.collider.gameObject.GetComponentInParent<EnemyStats>().TakeDamage(1);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range, targetLayer))
            {
                Debug.Log("Enemy Hit");
                if (hit.collider.CompareTag("HorizontalLimb"))
                {
                    hit.collider.gameObject.GetComponent<LimbStat>().TakeDamage(1);
                 
                }
                else
                {
                    hit.collider.gameObject.GetComponentInParent<EnemyStats>().TakeDamage(1);
                }
            }
        }
        //ray = new Ray(transform.position, transform.forward);
        
        
    }

    
    
}
