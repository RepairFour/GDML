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
        Instantiate(decal, particleCollisionEvent.intersection, Quaternion.LookRotation(particleCollisionEvent.normal), particleCollisionEvent.colliderComponent.gameObject.transform);
    }
    // Update is called once per frame
}
