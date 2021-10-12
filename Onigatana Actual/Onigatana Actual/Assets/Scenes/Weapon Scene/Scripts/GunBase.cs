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

    protected int ammoLeft;
    //int bulletsFired;

    protected bool isShooting;
    protected bool isReadyToShoot;
    protected bool isReloading;

    protected ButtonControl clickShoot;
    protected ButtonControl secondaryClickShoot;
    protected bool isHoldingShoot;

    [Header("Secondary Fire Variables")]
    public float cooldown;
    public int secondaryBulletsPerTap;
    public float secondaryRange;
    protected float secondaryCooldownTimer = 0f;
    protected bool OnCooldown;


    [Header("Animation and Sound")]
    public Animator gunAnimator;
    public GameObject muzzleFlash;
    public GameObject secondaryMuzzleFlash;
    public Transform muzzleFlashPoint;

    protected int ShootingSound;
    protected AudioSource audioSource;

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

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    protected bool PrimaryFirePressed()
    {
        return clickShoot.isPressed;
    }
    protected bool SecondaryFirePressed()
    {
        return secondaryClickShoot.isPressed;
    }
    protected void HandleInput()
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
    protected virtual void PrimaryFire()
    {
        
    }

    protected virtual void SecondaryFire()
    {
        
    }
    protected IEnumerator PrimaryCooldown()
    {
        // Debug.Log("Fire Rate Started");
        yield return new WaitForSeconds(fireRate);
        // Debug.Log("Ready to Shoot");
        isReadyToShoot = true; 
    }
    protected IEnumerator SecondaryCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        OnCooldown = false;
    }
    protected IEnumerator Reload()
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
