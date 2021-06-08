using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ModernFightingStyleGem", menuName = "Zenith/Gems/FightingStyleGems/ModernFightingStyleGem")]

public class ModernFightingStyle : FightingStyleGem
{
    [Header("Combo 1 WaitTimes")]
    public float startTrackToSweepKick = 3f;
    public float sweepKickAtackSpeed = 3f;
    public float sweepKickToRoundHouse = 3f;
    public float roundHouseAttackSpeed = 3f;
    public float chargeFlyingKneeTime = 3f;
    public float flyingKneeAttackSpeed = 3f;

    [Header("Combo 2 WaitTimes")]
    public float hangTime = 3f;
    public float timeTillLeapAgain = 3f;


    //For leap
    [Header("Leap Variables")]
    [SerializeField] Vector2 leapPower;
    bool leap = false;
    int leapDirection = 0;

    [Header("Tremor Variables")]
    [SerializeField] Vector2 tremorPower;
    [SerializeField] GameObject tremor;
    int tremorDirection;
    bool spawnTremor;

    

    //Tracking stuff
    bool trackingPlayer = false;
    bool moveToLocation = false;
    bool moving = false;
    Vector2 locationToMoveTo;

    //Combo setters and getters
    public bool isCombo1Started { get => combo1Started; set => combo1Started = value; }
    public bool isCombo2Started { get => combo2Started; set => combo2Started = value; }
    public bool isCombo2Completed { get => combo2Completed; }
    public bool isCombo1Completed { get => combo1Completed; }

    //Movement setters and getters
    public bool isTrackingPlayer { get => trackingPlayer; }
    public bool isMoving { get => moving; set => moving = value; }
    public Vector2 getLocationToMoveTo { get => locationToMoveTo; }
    public bool isMovingToLocation { get => moveToLocation; set => moveToLocation = value; }

    //Leap setters and getters
    public bool isLeaping { get => leap;}
    public Vector2 getLeapPower { get => leapPower;}
    public int getLeapDirection { get => leapDirection;}

    //Tremor setters and getters
    public GameObject getTremor { get => tremor;}
    public int getTremorDirection { get => tremorDirection;}
    public Vector2 getTremorPower { get => tremorPower; }
    public bool isSpawnTremor { get => spawnTremor; set => spawnTremor = value; }
   

    //Combo checkers
    bool combo1Started = false;
    bool combo2Started = false;

    bool combo1Completed = false;
    bool combo2Completed = false;


    bool returnMoving()
    {
        return moving;
    }

    void OnEnable()
    {
        combo1Started = false;
        combo2Started = false;
        combo1Completed = false;
        combo2Completed = false;
        trackingPlayer = false;
        moving = false;
        leap = false;
        spawnTremor = false;
        leapDirection = 0;
    }


    void Awake()
    {
        combo1Started = false;
        combo2Started = false;
        combo1Completed = false;
        combo2Completed = false;
        trackingPlayer = false;
        moving = false;
        leap = false;
        spawnTremor = false;
        leapDirection = 0;
    }


    

    public IEnumerator Combo1(Zenith zen)
    {
        combo1Started = true;
        StartTrack(); //Start tracking the player horizontally 
        yield return new WaitForSeconds(startTrackToSweepKick);

        SweepKickOn(zen); //Perform the sweep kick attack
        yield return new WaitForSeconds(sweepKickAtackSpeed);
        SweepKickOff(zen);

        yield return new WaitForSeconds(sweepKickToRoundHouse);
        RoundhouseOn(zen); // Perform the roundhouse attack
        yield return new WaitForSeconds(roundHouseAttackSpeed);
        RoundhouseOff(zen); // Perform the roundhouse attack

        StopTrack(); //Stop the tracking the player
        
        ChargeFlyingKnee();//Begin charging the flying knee attack
        yield return new WaitForSeconds(chargeFlyingKneeTime);
        FlyingKneeOn(zen); //Perform the flying knee attack
        yield return new WaitForSeconds(flyingKneeAttackSpeed);
        FlyingKneeOff(zen);
        combo1Started = false;
        combo1Completed = true;
    }

    public IEnumerator Combo2(Zenith zen)
    {
        //combo2Started = true;
        ////Move Zenith into position
        //MoveToLocation(new Vector2(14f, -4.34f));
        //yield return new WaitWhile(isMoveToLocation);
        yield return new WaitForSeconds(.5f);
        ////Leap to the left
        //StartLeap(-1);
        //yield return new WaitWhile(zen.isGroundedWhile);
        //EndLeap();
        //yield return new WaitUntil(zen.isGroundedUntil);
        ////Create and fire off the tremor
        //SpawnTremor(1);
        //yield return new WaitWhile(isTremorSpawn);

        //yield return new WaitForSeconds(timeTillLeapAgain);
        //StartLeap(1);
        //yield return new WaitWhile(zen.isGroundedWhile);
        //EndLeap();
        //yield return new WaitUntil(zen.isGroundedUntil);
        ////Create and fire off the tremor
        //SpawnTremor(-1);
        //yield return new WaitWhile(isTremorSpawn);

        ////Finish Combo
        //combo2Started = false;
        //combo2Completed = true;
    }

    public IEnumerator Combo3(Zenith zen)
    {
        yield return null;
        //Move Zenith of Screen 

        //Set up and order for indicators 

        //Flash indicators in order

        //Attack 1
        //Attack 2
        //Attack 3
        //Attack 4

        //Reposition


    }

    //Coroutine waitWhiles and waitUntils
    bool isTremorSpawn()
    {
        return spawnTremor;
    }
    bool isMoveToLocation()
    {
        return moveToLocation;
    }


    #region Attacks
    //int direction - +1 for right, -1 for left 
    public void SpawnTremor(int direction)
    {
        spawnTremor = true;
        tremorDirection = direction;
        //var temp = Instantiate(tremor, zen.GetTremorSpawnPoint.transform.position, Quaternion.identity);

        //temp.GetComponent<MoveTremor>().setDirection = direction;
        //temp.GetComponent<MoveTremor>().setTremorPower = tremorPower;
        //temp.GetComponent<MoveTremor>().setMoving = true;
    }



    public void RoundhouseOn(Zenith zen)
    {
        zen.GetRoundHouseKickCollider.SetActive(true);
        //Animation for roundhouse kick
    }
    public void RoundhouseOff(Zenith zen)
    {
        zen.GetRoundHouseKickCollider.SetActive(false);
    }
 
    public void FlyingKneeOn(Zenith zen)
    {
        zen.GetFlyingKneeCollider.SetActive(true);
        //Run and animation that sends Zenith across the screen 
    }
    public void FlyingKneeOff(Zenith zen)
    {
        zen.GetFlyingKneeCollider.SetActive(false); //Turn off the flying knee Collider
    }

    public void SweepKickOn(Zenith zen)
    {
        zen.GetSweepKickCollider.SetActive(true);
        //Sweep Kick animation
        
    }
    public void SweepKickOff(Zenith zen)
    {
        zen.GetSweepKickCollider.SetActive(false);
    }

    public void ChargeFlyingKnee()
    {
        //Run some sort of animation for a period of time
        //This will be done by the artist 
    }
    #endregion


    #region Movement
    public void MoveToLocation(Vector2 locationToMove)
    {
        locationToMoveTo = locationToMove;
        moveToLocation = true;
    }


    public void StartLeap(int dir)
    {
        leap = true;
        leapDirection = dir;
        
    }
    public void EndLeap()
    {
        leap = false;
    }
    

    #endregion
    public void StartTrack()
    {
        trackingPlayer = true;
        //Sets the value for tracking the player, this tracking 
        //is actually handled withing the Zenith class that derives 
        //from boss stats
    }

    public void StopTrack()
    {
        trackingPlayer = false;
    }

}
