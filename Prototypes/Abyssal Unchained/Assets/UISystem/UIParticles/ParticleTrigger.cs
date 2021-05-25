using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    public new ParticleSystem particleSystem;
    public Camera orthoCamera;
    // Start is called before the first frame update
    void Start()
    {
        if(particleSystem == null)
		{
            Debug.Log("particleSystem on " + gameObject.name + " is empty");
		}
    }

    public void Invoke()
	{
        LineUpParticleSystem();
        particleSystem.Play();
    }

    private void LineUpParticleSystem()
	{
        var t = gameObject.transform.position;
        t.z -= 2;
        particleSystem.gameObject.transform.position = t;
    }
}
