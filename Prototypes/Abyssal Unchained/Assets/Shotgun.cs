using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] int waves;
    int wavesFired = 0;
    [SerializeField] float waveCooldown;
    float waveCDTimer = 0;
    [SerializeField] int bulletsPerWave;
    [SerializeField] float energyRequired;
    [SerializeField] GameObject bullet;
    [SerializeField] float speed;
    [SerializeField] Transform ShotgunEnd;
    [SerializeField] float cooldown;
    float cooldownTimer;
    bool firing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(energyRequired <= Player.instance.WeaponEnergy() 
            && cooldownTimer < 0 
            && Input.GetMouseButtonDown(1))
		{
            firing = true;
            cooldownTimer = cooldown;
            Player.instance.UseWeaponEnergy(energyRequired);            
		}
        else
		{
            cooldownTimer -= Time.deltaTime;
        }

        if(firing)
		{
            waveCDTimer += Time.deltaTime;
            if(waveCDTimer >= waveCooldown)
			{
                waveCDTimer = 0;
                Fire();
                ++wavesFired;

            }
            if(wavesFired == waves)
			{
                firing = false;
                wavesFired = 0;
			}
		}
    }

    private void Fire()
	{           
        for (int x = 0; x < bulletsPerWave; ++x)
        {
            Quaternion rotation = ShotgunEnd.rotation;
            rotation.z -= 0.1f * x;
            GameObject go = Instantiate(bullet, ShotgunEnd.position, rotation);
            go.GetComponent<Rigidbody2D>().velocity = go.transform.right * speed;
        }     		        
	}
}
