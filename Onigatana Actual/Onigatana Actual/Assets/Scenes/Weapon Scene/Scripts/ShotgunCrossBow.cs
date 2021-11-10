using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunCrossBow : GunBase
{
    public ParticleSystem blood;
    public LayerMask layersToAvoid;
    // Start is called before the first frame update
    protected override void PrimaryFire() {

        GameManager.instance.bloodFuryState.DrainBloodMeter(12);
        isReadyToShoot = false;
        //Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        for (int i = 0; i < bulletsPerTap; i++)
        {
            var spreadVector = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));

            //Debug.Log(fpsCamera.transform.forward + new Vector3(0.1f,0 , 0.1f));

            Ray ray = new Ray(fpsCamera.gameObject.transform.position, Vector3.Normalize(fpsCamera.transform.forward + spreadVector));
            RaycastHit hit;

            //Debug.Log("Firing");
            if (Physics.Raycast(ray, out hit, range, ~layersToAvoid, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Debug.Log("Enemy hit");
                        hit.collider.GetComponent<EnemyStats>().Hurt((int)dmgPerBullet, EnemyStats.MeleeAnimation.ANIMATION1);
                        var bloodPoint = Instantiate(blood, hit.point, Quaternion.identity);
                    }
                    else
                    {
                        Debug.Log("Hit Something");
                    }
                    var h = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);

                }

            }
            var temp = Instantiate(shotHit, muzzleFlashPoint.position, Quaternion.LookRotation(fpsCamera.transform.forward + spreadVector));

        }
        if (muzzleFlash != null)
        {
            var newMuzzleFlash = Instantiate(muzzleFlash, muzzleFlashPoint.position, Quaternion.identity);
            newMuzzleFlash.transform.parent = gameObject.transform;
            //hasMuzzleFlashOn = true;

        }
        ShootingSound = Random.Range(1, 11);

        if (ShootingSound <= 5)
        {
            audioSource.PlayOneShot(gunShotSFX, 0.7f);

        }
        else
        {
            audioSource.PlayOneShot(gunShotSFXAlt, 0.7f);
        }

        gunAnimator.SetBool("Fire", true);
        //gunAnimator.SetFloat("FireLength", fireRate);
        ammoLeft--;
        //StartCoroutine(PrimaryCooldown());
        
    }

    protected override void SecondaryFire()
    {
        for (int i = 0; i < secondaryBulletsPerTap; i++)
        {


            Ray ray = new Ray(fpsCamera.gameObject.transform.position, Vector3.Normalize(fpsCamera.transform.forward));
            RaycastHit hit;

            //Debug.Log("Firing");
            if (Physics.Raycast(ray, out hit, secondaryRange,~LayerMask.NameToLayer("Ignore Raycast"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Debug.Log("Enemy hit");
                        hit.collider.gameObject.GetComponent<EnemyStats>().Hurt(0, EnemyStats.MeleeAnimation.ANIMATION1);
                        hit.collider.gameObject.GetComponent<Mark>().SetMark(Mark.Marks.BLINK);

                    }
                    else
                    {
                        Debug.Log("Hit Something");
                    }
                    var h = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);


                }

            }
            var temp = Instantiate(shotHit, muzzleFlashPoint.position, Quaternion.LookRotation(fpsCamera.transform.forward));
        }
        if (secondaryMuzzleFlash != null)
        {
            var newMuzzleFlash = Instantiate(secondaryMuzzleFlash, muzzleFlashPoint.position, Quaternion.identity);
            newMuzzleFlash.transform.parent = gameObject.transform;
            //hasMuzzleFlashOn = true;

        }
        ShootingSound = Random.Range(1, 11);

        if (ShootingSound <= 5)
        {
            audioSource.PlayOneShot(secondaryFireSFX, 0.2f);

        }
        else
        {
            audioSource.PlayOneShot(secondaryFireSFXAlt, 0.2f);
        }

        gunAnimator.SetBool("SecondaryFire", true);
        OnCooldown = true;
        StartCoroutine(SecondaryCooldown());
    }
}
