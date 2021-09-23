using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;


public class GunTest : MonoBehaviour
{

    PlayerMap input;

    public GameObject bullet;
    public float shootForce, upwardForce;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;
    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;

    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    public AudioClip gunShotSFX;
    public AudioClip reloadSFX;

    public Animator gunAnimator;

    private bool hasMuzzleFlashOn;



    public bool allowInvoke = true;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        input = new PlayerMap();
        input.Enable();
    }

    private void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = input.Player.Shoot.triggered;

        if (input.Player.Reload.triggered && bulletsLeft < magazineSize && !reloading) 
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) 
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        Debug.Log("bulletFired");
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        float x = Random.Range(-spread, spread);
        Debug.Log("X Spread Value = " + x);
        float y = Random.Range(-spread, spread);
        Debug.Log("T Spread Value = " + y);
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;


        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);


        if (hasMuzzleFlashOn == false) {
            if (muzzleFlash != null)
            {

                var newMuzzleFlash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
                newMuzzleFlash.transform.parent = gameObject.transform;
                hasMuzzleFlashOn = true;

            }
            gameObject.GetComponent<AudioSource>().PlayOneShot(gunShotSFX, 0.1f);
        }

        bulletsLeft--;
        bulletsShot++;



        if (allowInvoke)
        {
            gunAnimator.SetTrigger("Fire");
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
            hasMuzzleFlashOn = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

      

    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
        //this is only for the shotgun's functionality
        if (bulletsLeft <= 0)
        {
            Reload();
        }


    }

    private void Reload()
    {
        gunAnimator.SetTrigger("Reload");
        reloading = true;
        gameObject.GetComponent<AudioSource>().PlayOneShot(reloadSFX, 2);
        Invoke("ReloadFinished", reloadTime);

    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;

    }
}
