using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnims : MonoBehaviour
{

    public Animator enemyTestAnims;

    private bool InAnimation1;
    private bool InAnimation2;

    public Material defaultMaterial;
    public Material hurtMaterial;

    private void OnCollisionEnter(Collision collidedObject)
    {
        if (collidedObject.collider.tag == "MeleeAttack" && InAnimation1 == false)
        {
            gameObject.GetComponent<Animator>().enabled = true;
            InAnimation1 = true;
            InAnimation2 = false;
            gameObject.GetComponent<MeshRenderer>().material = hurtMaterial;
            
            Debug.Log("Collided");
            enemyTestAnims.SetTrigger("Attacked1");
            CancelInvoke("resetToNormal");


            Invoke("resetToNormal", 0.5f);
            Invoke("resetMaterial", 0.1f);

        }

        else if (collidedObject.collider.tag == "MeleeAttack"  && InAnimation1 == true)
        {
            gameObject.GetComponent<Animator>().enabled = true;
            InAnimation1 = false;
            InAnimation2 = true;
            gameObject.GetComponent<MeshRenderer>().material = hurtMaterial;

            Debug.Log("Collided");
            enemyTestAnims.SetTrigger("Attacked2");
            CancelInvoke("resetToNormal");


            Invoke("resetToNormal", 0.35f);
            Invoke("resetMaterial", 0.1f);
        }

    }

    private void resetToNormal()
    {
        gameObject.GetComponent<Animator>().enabled = false;
        InAnimation1 = false;
        InAnimation2 = true;
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        enemyTestAnims.SetTrigger("Reset");
    }

    private void resetMaterial()
    {
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}
