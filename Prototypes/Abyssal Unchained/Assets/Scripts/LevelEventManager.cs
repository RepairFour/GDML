using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelEventManager : MonoBehaviour
{
    
    public static LevelEventManager instance;
    private Scene thisScene;

    private int replaysAfterLoss = 0;
    private int replaysAfterWin = 0;
    private int deaths = 0;
    private int defeatedBoss = 0;

    int sessions;
    float sessionTime;
    Dictionary<string, object> playTimes = new Dictionary<string, object>();

    private float totalSessionTime;

    private bool timerStarted;
    
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            thisScene = SceneManager.GetActiveScene();

            AnalyticsEvent.LevelStart(thisScene.name, thisScene.buildIndex);
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        
    }

    public void IncrementReplayAfterLoss()
    {
        replaysAfterLoss++;
    }
    public void IncrementReplayAfterWin()
    {
        replaysAfterWin++;
    }
    public void IncrementDeaths()
    {
        deaths++;
    }
    public void IncrementBossKills()
    {
        defeatedBoss++;
    }

    public void ToggleTimer(bool a)
    {
        timerStarted = a;
    }

    public void ResetTimer()
    {
        playTimes.Add(sessions.ToString(), sessionTime);
        sessions++;
        sessionTime = 0;
        ToggleTimer(false);
    }

    private void Update()
    {

        totalSessionTime += Time.deltaTime;
        if (timerStarted)
        {
            sessionTime += Time.deltaTime;
        }

        

    }

    private void OnDestroy()
    {
        Dictionary<string, object> customParams = new Dictionary<string, object>();

        customParams.Add("Replays after loss", replaysAfterLoss);
        customParams.Add("Replays after win", replaysAfterWin);
        customParams.Add("Deaths", deaths);
        customParams.Add("Defeated Boss", defeatedBoss);
        customParams.Add("Session Time", totalSessionTime);

        Debug.Log("Loss Replays " + customParams["Replays after loss"]);
        Debug.Log("Replays after win " + customParams["Replays after win"]);
        Debug.Log("Deaths " + customParams["Deaths"]);
        Debug.Log("Defeated Boss" + customParams["Defeated Boss"]);
        Debug.Log("Session Time" + customParams["Session Time"]);


        AnalyticsEvent.Custom("User Data", customParams);

        AnalyticsEvent.Custom("PlayTime Data", playTimes);

        //Debug.Log(customParams["Replays after loss"]);
        for (int i = 0; i < playTimes.Count; i++)
        {
            Debug.Log(playTimes[i.ToString()]);
        }
        
    }
}
