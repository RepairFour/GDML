using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//deprecated
public class EnemyAnims : MonoBehaviour
{

  //  public Animator enemyTestAnims;

  //  private bool InAnimation1;
  //  private bool InAnimation2;

  //  private GameObject Player;
  //  private Rigidbody enemyRigidbody;
  //  private float knockbackStrength;

  //  public Vector3 knockBackDirection;

  //  public Material defaultMaterial;
  //  public Material hurtMaterial;



  //  private void Start()
  //  {
  //      Player = GameObject.FindGameObjectWithTag("Player");
  //      enemyRigidbody = gameObject.GetComponent<Rigidbody>();
  //  }

  //  public void EnemyAnimateHit(int animation)
  //  {
  //      gameObject.GetComponent<Animator>().enabled = true;
  //      if (animation == 1)
  //      {
  //          InAnimation1 = true;
  //          InAnimation2 = false;
  //          enemyTestAnims.SetTrigger("Attacked1");
  //      }
  //      else if(animation == 2)
		//{
  //          InAnimation1 = false;
  //          InAnimation2 = true;
  //          enemyTestAnims.SetTrigger("Attacked2");
  //      }
  //      gameObject.GetComponent<MeshRenderer>().material = hurtMaterial;


  //      //Here we calculate from what position the player hits the enemy, in order to calculate what direction the knockback will trigger in later. We also increase the knockback strength, but it is capped at a certain amount.
  //      //enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
  //      knockBackDirection = enemyRigidbody.transform.position - Player.transform.position;
  //      knockbackStrength = knockbackStrength + 150;
  //      if (knockbackStrength >= 650)
  //      {
  //          knockbackStrength = 650;
  //      }

  //      //We activate their animation, and we cancel any reset invokes. We do this so that we can continually keep the enemy in hitstun/animations until we stop attacking, and then we send them back to normal.
        
  //      CancelInvoke("resetToNormal");

  //      //After all this, we reset their entire state back to normal in a few frames (only slightly longer than the max attack speed of the sword). This gets cancelled if another attack is launched, in order to allow players to continually attack the same enemy.
  //      //We reset materials after only 1/10th of a second, to give a satisfying hit effect.
  //      Invoke("resetToNormal", 0.25f);
  //      Invoke("resetMaterial", 0.1f);


  //  }

    
  //  //There's a pretty horrible bug with the collisions that causes them to not work if the player's attack hitbox is already inside the enemy object when it's turned on. This makes it really annoying to hit enemies.
  //  //I think we should handle this better than collisionenter, but its my best solution for now.
  //  private void OnTriggerEnter (Collider collidedObject)
  //  {
  //      //There is likely a better way to handle this, since colliders seem to make the hitboxes rather buggy.
  //      //We check to see if the collider has the MeleeAttack tag, and if it does, and the enemy isnt currently in their first hit animation, this part plays.
       
  //      if (collidedObject.tag == "MeleeAttack" && InAnimation1 == false)
  //      {
  //          EnemyHit(1);
  //      }

  //      //This function is virtually identical to the one above, but it allows us to have two different hit states, in the event that we want to have different hurt animations.
  //      else if (collidedObject.tag == "MeleeAttack"  && InAnimation1 == true)
  //      {
  //          EnemyHit(2);
  //      }
  //  }

  //  private void resetToNormal()
  //  {

  //      //When we reset back to normal, we clear the states, change the material back and reset the animation to default. We also calculate, perform and reset knockback, finally disabling the animator.
        
  //      InAnimation1 = false;
  //      InAnimation2 = false;
  //      gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
  //      enemyTestAnims.SetTrigger("Reset");

  //      Debug.Log(knockbackStrength);
  //      //enemyRigidbody.constraints = ~RigidbodyConstraints.FreezePositionX | ~RigidbodyConstraints.FreezePositionZ;
  //      enemyRigidbody.AddForce(knockBackDirection.normalized * knockbackStrength);
  //      knockbackStrength = 0;
  //      gameObject.GetComponent<Animator>().enabled = false;
        
  //  }


  //  //Simple invoked function to reset the material after a short timer.
  //  private void resetMaterial()
  //  {
  //      gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
  //  }
}
