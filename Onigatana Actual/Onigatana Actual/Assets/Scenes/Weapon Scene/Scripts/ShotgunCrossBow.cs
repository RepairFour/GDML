using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunCrossBow : GunBase
{
    // Start is called before the first frame update
    protected override void PrimaryFire() {
        isReadyToShoot = false;

        //Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        for (int i = 0; i < bulletsPerTap; i++)
        {
            var spreadVector = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));

            //Debug.Log(fpsCamera.transform.forward + new Vector3(0.1f,0 , 0.1f));

            Ray ray = new Ray(fpsCamera.gameObject.transform.position, Vector3.Normalize(fpsCamera.transform.forward + spreadVector));
            RaycastHit hit;

            //Debug.Log("Firing");
            if (Physics.Raycast(ray, out hit, range))
            {
                if (hit.collider)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Debug.Log("Enemy hit");
                        hit.collider.GetComponent<EnemyStats>().Hurt((int)dmgPerBullet, EnemyStats.MeleeAnimation.ANIMATION1);
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
            audioSource.PlayOneShot(gunShotSFX, 0.1f);

        }
        else
        {
            audioSource.PlayOneShot(gunShotSFXAlt, 0.1f);
        }

        gunAnimator.SetTrigger("Fire");
        ammoLeft--;
        StartCoroutine(PrimaryCooldown());

    }

    protected override void SecondaryFire()
    {
        for (int i = 0; i < secondaryBulletsPerTap; i++)
        {


            Ray ray = new Ray(fpsCamera.gameObject.transform.position, Vector3.Normalize(fpsCamera.transform.forward));
            RaycastHit hit;

            //Debug.Log("Firing");
            if (Physics.Raycast(ray, out hit, secondaryRange))
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
            audioSource.PlayOneShot(secondaryFireSFX, 0.075f);

        }
        else
        {
            audioSource.PlayOneShot(secondaryFireSFXAlt, 0.075f);
        }

        gunAnimator.SetTrigger("SecondaryFire");
        OnCooldown = true;
        StartCoroutine(SecondaryCooldown());
    }
}
