using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zenith : BossStats
{
    [Header("Unique Sockets", order = 3)]

    [SerializeField] FightingStyleGem beta;
    [SerializeField] BrainGem omega;

    [Header("Attack Colliders")]
    [SerializeField] GameObject sweepKickCollider;
    [SerializeField] GameObject roundHouseKickCollider;
    [SerializeField] GameObject flyingKneeCollider;

    [Header("Attack SpawnPoints")]
    [SerializeField] GameObject leftTremorSpawnPoint;
    [SerializeField] GameObject rightTremorSpawnPoint;

    
    

    Rigidbody2D rb;
    bool grounded = false;

    bool trackingPlayer = false;
    bool movingToLocation = false;

    

    //Movement Variables
    float speed = 20f;

    float comboTimer = 0f;
    [Header("Combo 1 WaitTimes")]
    public float startTrackToSweepKick = 3f;
    public float sweepKickAtackSpeed = 3f;
    public float sweepKickToRoundHouse = 3f;
    public float roundHouseAttackSpeed = 3f;
    public float chargeFlyingKneeTime = 3f;
    public float flyingKneeAttackSpeed = 3f;

    [Header("Combo 2 Variables")]
    public Vector2 leapStart;
    bool movingToLocationComplete = false;
    bool leaping;
    bool leaped = false;
    bool tremorSpawned = false;
    int leapDirection;
    int tremorDirection;
    
    [SerializeField] Vector2 leapPower;
    [SerializeField] Vector2 tremorPower;
    [SerializeField] GameObject tremorObject;

    [Header("Combo 2 Timers")]
    public float timeTillLeapAfterTremor = 3f;
    public float WaitToLeapTime = 3f;

    [Header("Combo3 Indicators")]
    [SerializeField] List<GameObject> indicators = new List<GameObject>();
    public List<GameObject> orderOfIndicators = new List<GameObject>();
    bool indicatorsRandomised = false;

    [Header("Combo3 Variables")]
    [SerializeField] float dashSpeed = 100;

    int indicatorNumber = 0;
    bool intialPositionSet = false;
    bool velocitySet = false;
    Vector3 currentAttackStart;
    Vector3 currentAttackEnd;

    GameObject currentDashObject;
    bool dashInstantiated = false;

    [SerializeField] GameObject dash;
    

    bool movingOffScreen = false;
    bool movingOnScreen = false;
    
    [SerializeField] Vector3 offScreenTarget;
    [SerializeField] Vector3 onScreenTarget;

    [Header("Combo3 Timers")]
    [SerializeField] float indicatorFlashTime = 2f;


    //bool sweepKickDone = false;
    //bool roundHouseKickDone = false;
    //bool flyingKickChargeDone = false;
    //bool flyingKickDone = false;

    public enum Combo1Enum { Start,StartTrack, SweepKick, RoundHouse, StopTrack, ChargeFlyingKnee, FlyingKnee, ComboFinished}
    public Combo1Enum combo1Enum;

    public enum Combo2Enum { Start,MoveToLocation, WaitToLeap, LeapLeft, LeapingLeft, SpawnTremorRight, LeapRight, LeapingRight, SpawnTremorLeft, ComboFinished }
    public Combo2Enum combo2Enum;

    public enum Combo3Enum {Start, DisappearOfScreen ,RandomiseIndicators, FlashIndicator, IndicatorAttack, ReappearOnScreen, ComboFinished}
    public Combo3Enum combo3Enum;
    


    public GameObject GetSweepKickCollider { get => sweepKickCollider; }
    public GameObject GetRoundHouseKickCollider { get => roundHouseKickCollider; }
    public GameObject GetFlyingKneeCollider { get => flyingKneeCollider; }
    public bool isGrounded { get => grounded; set => grounded = value; }

    public Rigidbody2D getRb { get => rb;}

    public GameObject getLeftTremorSpawnPoint { get => leftTremorSpawnPoint; }
    public GameObject getRightTremorSpawnPoint { get => rightTremorSpawnPoint; }

    //Debugging

    bool combo1done = false;
    bool combo2done = false;
    private bool moving = false;

    private void Start()
    {
        if(beta is ModernFightingStyle)
        {
            var b = beta as ModernFightingStyle;
            b.isCombo1Started = false;
        }
        baseHealth = 200;
        prefix = alpha.ReturnPrefix();
        title = omega.ReturnTitle();
        postfix = beta.ReturnPostFix();
        rb = transform.parent.GetComponent<Rigidbody2D>();
        nameToPrint = prefix + " " + title + " " + postfix;
    }


    private void Update()
    {
        if(beta is ModernFightingStyle)
        {
            var b = beta as ModernFightingStyle;
            RunAI(Time.deltaTime);
            TrackPlayer(b);
            //RunCombo1(b);
            //if (!b.isCombo1Started)
            //{
            //    RunCombo2(b);
            //}
            //
            //CheckIfAllCombosComplete(b);
            //
            //

            //CheckSpawnTremor(b);
        }
       
    }

    private void RunAI(float deltaTime)
    {
        Combo1(deltaTime);
        if(combo1Enum == Combo1Enum.ComboFinished)
        {
            Combo2(deltaTime);
            if(combo2Enum == Combo2Enum.ComboFinished)
            {
                Combo3(deltaTime);
                if(combo3Enum == Combo3Enum.ComboFinished)
                {
                    combo1Enum = Combo1Enum.Start;
                    combo2Enum = Combo2Enum.Start;
                    combo3Enum = Combo3Enum.Start;
                }
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (beta is ModernFightingStyle)
        {
            var b = beta as ModernFightingStyle;
            MoveToLocation();
            if (leaping && !leaped)
            {
                var power = leapPower;
                power.x = power.x * leapDirection;
                
                rb.AddForce(power, ForceMode2D.Impulse);
                grounded = false;
                leaped = true;
            }
        }
    }


    #region ModernFightingStyle
    #region Combo1Attacks

    public void Combo1(float deltaTime)
    {
        comboTimer += deltaTime;
        switch (combo1Enum)
        {
            case Combo1Enum.Start:
                combo1Enum = Combo1Enum.StartTrack;
                break;
            case Combo1Enum.StartTrack:
                if (TrackPlayerOn())
                {
                    combo1Enum = Combo1Enum.SweepKick;
                    comboTimer = 0f;
                }
                break;
            case Combo1Enum.SweepKick:
                if (SweepKick())
                {
                    combo1Enum = Combo1Enum.RoundHouse;
                    comboTimer = 0f;
                }
                break;
            case Combo1Enum.RoundHouse:
                if (Roundhouse())
                {
                    combo1Enum = Combo1Enum.StopTrack;
                    comboTimer = 0f;
                }
                break;
            case Combo1Enum.StopTrack:
                trackingPlayer = false;
                combo1Enum = Combo1Enum.ChargeFlyingKnee;
                break;

            case Combo1Enum.ChargeFlyingKnee:
                if (ChargeFlyingKnee())
                {
                    combo1Enum = Combo1Enum.FlyingKnee;
                    comboTimer = 0f;
                }
                break;

            case Combo1Enum.FlyingKnee:
                if (FlyingKnee())
                {
                    combo1Enum = Combo1Enum.ComboFinished;
                    comboTimer = 0f;
                }
                break;
            case Combo1Enum.ComboFinished:
                break;
        }

    }

    private bool TrackPlayerOn()
    {
        trackingPlayer = true;
        if(comboTimer > startTrackToSweepKick)
        {
            return true;
        }
        return false;
    }

    public bool Roundhouse()
    {
        GetRoundHouseKickCollider.SetActive(true);
        if (comboTimer > roundHouseAttackSpeed)
        {
            GetRoundHouseKickCollider.SetActive(false);
            return true;
        }
        return false;

        
        //Animation for roundhouse kick
    }

    public bool FlyingKnee()
    {
        GetFlyingKneeCollider.SetActive(true);
        if(comboTimer > flyingKneeAttackSpeed)
        {
            GetFlyingKneeCollider.SetActive(false);
            return true;
        }
        return false;
        
    }

    public bool SweepKick()
    {
        GetSweepKickCollider.SetActive(true);
        if(comboTimer > sweepKickAtackSpeed)
        {
            GetSweepKickCollider.SetActive(false);
            return true;
        }
        return false;
    }

    public bool ChargeFlyingKnee()
    {
        if(comboTimer > chargeFlyingKneeTime)
        {
            return true;
        }
        return false;
    }
    #endregion
    
    #region Combo2Attacks
    void Combo2(float deltaTime)
    {
        comboTimer += deltaTime;
        switch (combo2Enum)
        {
            case Combo2Enum.Start:
                combo2Enum = Combo2Enum.MoveToLocation;
                break;
            case Combo2Enum.MoveToLocation:
                if(movingToLocation == false)
                {
                    movingToLocation = true;
                    moving = true;
                }
                if (movingToLocationComplete)
                {
                    combo2Enum = Combo2Enum.WaitToLeap;
                    comboTimer = 0;
                    movingToLocation = false;
                    movingToLocationComplete = false;

                    
                }
                break;

            case Combo2Enum.WaitToLeap:
                if(comboTimer >= WaitToLeapTime)
                {
                    combo2Enum = Combo2Enum.LeapLeft;
                }


                break;
            case Combo2Enum.LeapLeft:
                leaping = true;
                leapDirection = -1;
                combo2Enum = Combo2Enum.LeapingLeft;
                break;

            case Combo2Enum.LeapingLeft:

                if (grounded == true)
                {
                    combo2Enum = Combo2Enum.SpawnTremorRight;
                }
                break;


            case Combo2Enum.SpawnTremorRight:
                leaping = false;
                leaped = false;
                tremorDirection = 1;
                if (!tremorSpawned)
                {
                    SpawnTremor();
                    comboTimer = 0;
                }
                
                if(comboTimer >= timeTillLeapAfterTremor)
                {
                    combo2Enum = Combo2Enum.LeapRight;
                    tremorSpawned = false;
                }
                break;
            case Combo2Enum.LeapRight:

                leaping = true;
                leapDirection = 1;
                combo2Enum = Combo2Enum.LeapingRight;
                grounded = false;
                break;

            case Combo2Enum.LeapingRight:
                if (grounded == true)
                {
                    combo2Enum = Combo2Enum.SpawnTremorLeft;
                }
                break;

            case Combo2Enum.SpawnTremorLeft:
                leaping = false;
                leaped = false;
                tremorDirection = -1;
                if (!tremorSpawned)
                {
                    SpawnTremor();
                    comboTimer = 0;
                }
                
                if (comboTimer >= timeTillLeapAfterTremor)
                {
                    combo2Enum = Combo2Enum.ComboFinished;
                    tremorSpawned = false;
                }
                break;
                

            case Combo2Enum.ComboFinished:
              
                break;

        }
    }


    public void SpawnTremor()
    {
        GameObject temp = null;
        Vector3 position = new Vector3();

        if (tremorDirection == 1)
        {

            position = getRightTremorSpawnPoint.transform.position;

        }
        if (tremorDirection == -1)
        {
            position = getLeftTremorSpawnPoint.transform.position;
        }
        temp = Instantiate(tremorObject, position, Quaternion.identity);

        temp.GetComponent<MoveTremor>().setDirection = tremorDirection;
        temp.GetComponent<MoveTremor>().setTremorPower = tremorPower;
        temp.GetComponent<MoveTremor>().setMoving = true;
        Debug.Log("Tremor Spawned");
        tremorSpawned = true;

    }


    #endregion

    #region Combo3Attacks

    public void Combo3(float deltaTime)
    {
        comboTimer += deltaTime;
        switch (combo3Enum)
        {
            case Combo3Enum.Start:
                combo3Enum = Combo3Enum.DisappearOfScreen;
                break;
            case Combo3Enum.DisappearOfScreen:
                if(movingOffScreen == false)
                {
                    moving = true;
                    movingOffScreen = true;
                    //movingToLocationComplete = false;
                }
                if (movingToLocationComplete)
                {
                    combo3Enum = Combo3Enum.RandomiseIndicators;
                    comboTimer = 0;
                    movingToLocationComplete = false;
                    movingOffScreen = false;
                }
                break;

            case Combo3Enum.RandomiseIndicators:
                RandomiseIndicatorList();
                if (indicatorsRandomised)
                {
                    combo3Enum = Combo3Enum.FlashIndicator;
                    comboTimer = 0f;
                }
                break;
            //

            case Combo3Enum.FlashIndicator:
                orderOfIndicators[indicatorNumber].SetActive(true);

                if (comboTimer > indicatorFlashTime)
                {
                    orderOfIndicators[indicatorNumber].SetActive(false);
                    
                    indicatorNumber++;
                    comboTimer = 0f;
                    if(indicatorNumber > 3)
                    {
                        combo3Enum = Combo3Enum.IndicatorAttack;
                        indicatorNumber = 0;
                    }
                }
                break;

            case Combo3Enum.IndicatorAttack:

                if (IndicatorAttacks())
                {
                    indicatorNumber++;
                    if(indicatorNumber > 3)
                    {
                        combo3Enum = Combo3Enum.ComboFinished;
                        indicatorNumber = 0;
                    }
                }
                
                break;
            case Combo3Enum.ReappearOnScreen:
                if (movingOnScreen == false)
                {
                    moving = true;
                    movingOnScreen = true;
                    //movingToLocationComplete = false;
                }
                if (movingToLocationComplete)
                {
                    combo3Enum = Combo3Enum.ComboFinished;
                    comboTimer = 0;
                    movingToLocationComplete = false;
                    movingOnScreen = false;
                }

                break;
            case Combo3Enum.ComboFinished:


                break;



        }

    }

    bool IndicatorAttacks()
    {
        if (!dashInstantiated)
        {
            
            var temp = orderOfIndicators[indicatorNumber];

            currentAttackStart = temp.transform.GetChild(0).transform.position;
            currentAttackEnd = temp.transform.GetChild(1).transform.position;

            var attackDirection = currentAttackEnd - currentAttackStart;
            attackDirection.Normalize();

            currentDashObject = Instantiate(dash, currentAttackStart, Quaternion.identity);

            var moveDash = currentDashObject.GetComponent<MoveDash>();

            moveDash.SetSpeed(dashSpeed);
            moveDash.SetDirection(attackDirection);
            moveDash.SetVelocity();

            dashInstantiated = true;

        }

        if (currentDashObject.GetComponent<MoveDash>().isObjectFinished) {

            dashInstantiated = false;
            Destroy(currentDashObject);
            return true;
        }

        return false;

        
    }

    void RandomiseIndicatorList()
    {
        System.Random r = new System.Random();
        List<int> usedInts = new List<int>();

        for (int v = 0; v < 4; v++)
        {
            int i = r.Next(0, 4);
            while (usedInts.Contains(i) && usedInts.Count < 4)
            {
                i = r.Next(0, 4);
            }
            usedInts.Add(i);
            orderOfIndicators.Add(indicators[i]); 
        }

        indicatorsRandomised = true;
        

    }



    #endregion


    #region MovementFunctions
    public void MoveToLocation()
    {
        Vector3 targetPosition = new Vector3();

        if (movingToLocation)
        {
            targetPosition = leapStart;
        }
        if (movingOffScreen)
        {
            targetPosition = offScreenTarget;
        }
        if (movingOnScreen)
        {
            targetPosition = onScreenTarget;
        }

        if (moving && (movingToLocation || movingOffScreen || movingOnScreen))
        {
            var currentPosition = gameObject.transform.parent.position;
            

            Vector3 directionToMove = targetPosition - currentPosition;
            directionToMove.Normalize();

            rb.velocity = directionToMove * speed;

            //gameObject.transform.parent.position = Vector3.Lerp(currentPosition, targetPosition, 0.2f);

            if(Mathf.Abs(gameObject.transform.parent.position.x - targetPosition.x) < 0.5)
            {
                rb.velocity = new Vector3(0, 0, 0);
                //movingToLocation = false;
                //movingOffScreen = false;
                movingToLocationComplete = true;
                //moving = false;

                
            }
        }
    }

    public void TrackPlayer(ModernFightingStyle b)
    {
        if (trackingPlayer)
        {
            var currentPosition = transform.parent.position;
            Vector3 targetPosition = new Vector3(LevelManager.instance.player.gameObject.transform.position.x + 8, currentPosition.y, currentPosition.z);

            gameObject.transform.parent.position = Vector3.Lerp(currentPosition, targetPosition, 0.2f);
        }
    }
    #endregion

    #endregion

}


