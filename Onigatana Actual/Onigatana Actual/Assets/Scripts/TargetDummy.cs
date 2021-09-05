using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(ParticleSystem))]
public class TargetDummy : MonoBehaviour
{
    [SerializeField] Animation hitAni;
    [SerializeField] ParticleSystem blood;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        hitAni.Play("Death");
        blood.Play();

    }
}
