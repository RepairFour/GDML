using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;


public class GunTest : MonoBehaviour
{
    
    //Hey Josh and Cameron, if you're looking at this code, I'm really fucking sorry.
    //It's a total mess.

    PlayerMap input;

    public GameObject bullet;
    public GameObject secondaryBullet;
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
    public AudioClip gunShotSFXAlt;
    public AudioClip secondaryFireSFX;
    public AudioClip click;
    private bool clickSoundCanPlay;
    public AudioClip reloadSFX;

    private int ShootSoundPicker;

    public Animator gunAnimator;

    private bool hasMuzzleFlashOn;

    private ButtonControl ClickShoot;
    private ButtonControl SecondaryClickShoot;
    private bool isHoldingShoot;

    public bool allowInvoke = true;

    private bool secondaryFireActive;
    public float secondaryFireDelayIncrease;
    public float secondaryFireProjectilesBonus;
    //public int secondaryFireAmmoConsumptionBonus;
    public float secondaryFireSpreadIncrease;
    public float secondaryFireShootForceBonus;
    private bool hasFiredPrimaryShot;
    public GameObject secondaryFireMuzzleFlash;

    private float initialSecondaryFireDelayIncrease;
    private float initialSecondaryFireProjectileBonus;
    //private int initialSecondaryFireAmmoConsumptionBonus;


    private bool queryShooting()
    {
        if (ClickShoot.isPressed)
        {
            isHoldingShoot = true;
            secondaryFireActive = false;
        }
        else if (ClickShoot.wasReleasedThisFrame)
        {
            isHoldingShoot = false;
            secondaryFireActive = false;
        }
        


        else if (SecondaryClickShoot.isPressed)
        {
            Debug.Log("SecondaryFireActive");
            isHoldingShoot = true;
            secondaryFireActive = true;
        }
        else if (SecondaryClickShoot.wasReleasedThisFrame)
        {
            Debug.Log("SecondaryFireDeactive");
            isHoldingShoot = false;
            secondaryFireActive = false;
        }



        return isHoldingShoot;
    }

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        hasFiredPrimaryShot = false;
        clickSoundCanPlay = true;

        input = new PlayerMap();
        input.Enable();

        ClickShoot = (ButtonControl)input.Player.Shoot.controls[0];
        //SecondaryClickShoot = (ButtonControl)input.Player.SecondaryFire.controls[0];


       //initialSecondaryFireAmmoConsumptionBonus = secondaryFireAmmoConsumptionBonus;
        initialSecondaryFireDelayIncrease = secondaryFireDelayIncrease;
        initialSecondaryFireProjectileBonus = secondaryFireProjectilesBonus;

    }

    private void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);


    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = queryShooting();
        else shooting = input.Player.Shoot.triggered;



        //if (input.Player.Reload.triggered && bulletsLeft < magazineSize && !reloading) 
           // Reload();

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

        //resets the gun if ammo is not full, and the player tries to use the secondary fire, and plays a click sound effect.
        if (secondaryFireActive == true && hasFiredPrimaryShot == true && reloading == false)
        {
            readyToShoot = true;

            if (clickSoundCanPlay == true)
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(click, 1f);
                clickSoundCanPlay = false;
                Invoke("clickSoundCooldown", 0.5f);
            }
        
        }

        //handles primary fire
        else if (secondaryFireActive == false)
        {
            handleSecondaryFire(0);
            hasFiredPrimaryShot = true;
            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(75);

            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
            float x = Random.Range(-spread, spread);
            // Debug.Log("X Spread Value = " + x);
            float y = Random.Range(-spread, spread);
            // Debug.Log("T Spread Value = " + y);
            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;


            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);



            if (hasMuzzleFlashOn == false)
            {
                if (muzzleFlash != null)
                {

                    var newMuzzleFlash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
                    newMuzzleFlash.transform.parent = gameObject.transform;
                    hasMuzzleFlashOn = true;

                }

                ShootSoundPicker = Random.Range(1, 11);
                Debug.Log("ShootSoundPicker Value = " + ShootSoundPicker);

                if (ShootSoundPicker <= 5)
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(gunShotSFX, 0.1f);
                }
                else gameObject.GetComponent<AudioSource>().PlayOneShot(gunShotSFXAlt, 0.1f);

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


        //handles secondary fire
        else if (hasFiredPrimaryShot == false)
        {
            if (secondaryFireActive == true)
            {
                handleSecondaryFire(1);
                Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;

                Vector3 targetPoint;
                if (Physics.Raycast(ray, out hit))
                    targetPoint = hit.point;
                else
                    targetPoint = ray.GetPoint(75);

                Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
                float x = Random.Range(-(spread + secondaryFireSpreadIncrease), (spread + secondaryFireSpreadIncrease));
                // Debug.Log("X Spread Value = " + x);
                float y = Random.Range(-(spread + secondaryFireSpreadIncrease), (spread + secondaryFireSpreadIncrease));
                // Debug.Log("T Spread Value = " + y);
                Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

                GameObject secondaryCurrentBullet = Instantiate(secondaryBullet, attackPoint.position, Quaternion.identity);
                secondaryCurrentBullet.transform.forward = directionWithSpread.normalized;


                secondaryCurrentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * (shootForce + secondaryFireShootForceBonus), ForceMode.Impulse);
                secondaryCurrentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

                //secondaryFireActive = false;

                if (hasMuzzleFlashOn == false)
                {
                    if (secondaryFireMuzzleFlash != null)
                    {


                        var newMuzzleFlash = Instantiate(secondaryFireMuzzleFlash, attackPoint.position, Quaternion.identity);
                        var secondNewMuzzleFlash = Instantiate(secondaryFireMuzzleFlash, attackPoint.position, Quaternion.identity);
                        newMuzzleFlash.transform.parent = gameObject.transform;
                        secondNewMuzzleFlash.transform.parent = gameObject.transform;
                        hasMuzzleFlashOn = true;

                    }

                    ShootSoundPicker = Random.Range(1, 11);
                    Debug.Log("ShootSoundPicker Value = " + ShootSoundPicker);

                    if (ShootSoundPicker <= 5)
                    {
                        gameObject.GetComponent<AudioSource>().PlayOneShot(secondaryFireSFX, 0.075f);
                    }
                    else gameObject.GetComponent<AudioSource>().PlayOneShot(secondaryFireSFX, 0.075f);

                }

                bulletsLeft--;
                bulletsShot++;

                if (allowInvoke)
                {
                    gunAnimator.SetTrigger("SecondaryFire");
                    Invoke("ResetShot", (timeBetweenShooting+secondaryFireDelayIncrease)
                        );


                    allowInvoke = false;
                    hasMuzzleFlashOn = false;
                }

                if (bulletsShot < bulletsPerTap + secondaryFireProjectilesBonus && bulletsLeft > 0)
                    Invoke("Shoot", timeBetweenShots);
            }
        }

      
        


      

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
        Invoke("ReloadFinished", (reloadTime//+secondaryFireDelayIncrease
            ));

    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        hasFiredPrimaryShot = false;
        Debug.Log("bulletsLeft " + bulletsLeft);
        Debug.Log("magazineSize " + magazineSize);

    }

    private void handleSecondaryFire(int FireCheck)
    { 

        if (FireCheck == 1){
            //secondaryFireAmmoConsumptionBonus = initialSecondaryFireAmmoConsumptionBonus;
            secondaryFireDelayIncrease = initialSecondaryFireDelayIncrease;
            secondaryFireProjectilesBonus = initialSecondaryFireProjectileBonus;
        }
        else {
            //secondaryFireAmmoConsumptionBonus = 0;
            secondaryFireDelayIncrease = 0;
            secondaryFireProjectilesBonus = 0;
        }

    }

    private void clickSoundCooldown()
    {
        clickSoundCanPlay = true;
    }

 
}
