using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class GunBase : MonoBehaviour
{
    PlayerMap input;

    [Header("Primary Fire Variables")]
    public float fireRate;
    public float spread;
    public float range;
    public float burstTime;
    public float reloadTime;
    public float dmgPerBullet;

    public int magSize;
    public int bulletsPerTap;

    public Camera fpsCamera;

    int ammoLeft;
    //int bulletsFired;

    bool isShooting;
    bool isReadyToShoot;
    bool isReloading;

    private ButtonControl clickShoot;
    private ButtonControl secondaryClickShoot;
    bool isHoldingShoot;

    [Header("Secondary Fire Variables")]
    public float cooldown;
    public int secondaryBulletsPerTap;
    public float secondaryRange;
    private float secondaryCooldownTimer = 0f;
    private bool OnCooldown;


    [Header("Animation and Sound")]
    public Animator gunAnimator;
    public GameObject muzzleFlash;
    public GameObject secondaryMuzzleFlash;
    public Transform muzzleFlashPoint;

    int ShootingSound;
    AudioSource audioSource;

    public AudioClip gunShotSFX;
    public AudioClip gunShotSFXAlt;
    public AudioClip secondaryFireSFX;
    public AudioClip secondaryFireSFXAlt;
    public AudioClip click;
    public AudioClip reloadSFX;

    public ParticleSystem shotHit;
    public ParticleSystem bulletHole;
    public float bulletFxSpeed;



    private void Awake()
    {
        ammoLeft = magSize;
        isReadyToShoot = true;

        input = new PlayerMap();
        input.Enable();

        clickShoot = (ButtonControl)input.Player.Shoot.controls[0];
        secondaryClickShoot = (ButtonControl)input.Player.SecondaryFire.controls[0];
        isReadyToShoot = true;

        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private bool PrimaryFirePressed()
    {
        //if (clickShoot.isPressed)
        //{
        //    isHoldingShoot = true;
        //}
        //else if (clickShoot.wasReleasedThisFrame)
        //{
        //    isHoldingShoot = false;
        //}
        return clickShoot.isPressed;
    }
    private bool SecondaryFirePressed()
    {
        return secondaryClickShoot.isPressed;
    }

    private void HandleInput()
    {
       
        if (!isReloading && ammoLeft <= 0)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
        else if((isReadyToShoot && PrimaryFirePressed()) && !isReloading && ammoLeft > 0)
        {
            PrimaryFire();
            //Debug.Log("Shoot");
        }
        else if (isReadyToShoot && SecondaryFirePressed() && !OnCooldown && !isShooting && !isReloading)
        {
            SecondaryFire();
        }
    }

    private void PrimaryFire()
    {
        isReadyToShoot = false;

        //Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        for(int i = 0; i < bulletsPerTap; i++)
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
                        hit.collider.GetComponent<EnemyAnims>().EnemyHit();
                        hit.collider.GetComponent<EnemyStats>().Hurt((int)dmgPerBullet);
                        
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
        if(muzzleFlash != null)
        {
            var newMuzzleFlash = Instantiate(muzzleFlash, muzzleFlashPoint.position, Quaternion.identity);
            newMuzzleFlash.transform.parent = gameObject.transform;
            //hasMuzzleFlashOn = true;

        }
        ShootingSound = Random.Range(1, 11);

        if(ShootingSound <= 5)
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

    private void SecondaryFire()
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
                        hit.collider.gameObject.GetComponent<EnemyAnims>().EnemyHit();
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
   
    IEnumerator PrimaryCooldown()
    {
       // Debug.Log("Fire Rate Started");
        yield return new WaitForSeconds(fireRate);
       // Debug.Log("Ready to Shoot");
        isReadyToShoot = true; 
    }
    IEnumerator SecondaryCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        OnCooldown = false;
    }
    IEnumerator Reload()
    {
        gunAnimator.SetTrigger("Reload");
        gameObject.GetComponent<AudioSource>().PlayOneShot(reloadSFX, 2);
        // Debug.Log("Reloading");
        yield return new WaitForSeconds(reloadTime);
       // Debug.Log("Reloaded");
        isReloading = false;
        ammoLeft = magSize;
    }
}
