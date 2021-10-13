using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLauncher : MonoBehaviour
{
    public ParticleSystem particleLauncher;
    public GameObject decal;

    List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }
    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            EmitAtLocation(collisionEvents[i]);
        }
        
    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        var temp = Instantiate(decal, particleCollisionEvent.intersection, Quaternion.LookRotation(particleCollisionEvent.normal), particleCollisionEvent.colliderComponent.gameObject.transform);
        //var rotation = temp.transform.localRotation;
        //rotation.y = Random.Range(0, 360);
        //temp.transform.localRotation = rotation;
    }
    // Update is called once per frame
}
