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

    int runs;
    float runTime;
    

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
    
    public void RaiseReplayAfterLossEvent()
    {
        replaysAfterLoss++;
        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("Player Loss", replaysAfterLoss);
        AnalyticsResult result = AnalyticsEvent.Custom("Player Loss", customParams);
        Debug.Log("PlayerLoss " + result);
       
    }
    public void RaiseReplayAfterWinEvent()
    {
        replaysAfterWin++;
        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("Player Win", replaysAfterWin);
        AnalyticsResult result = AnalyticsEvent.Custom("Player Win", customParams);
        Debug.Log("PlayerWin" + result);
    }
    public void RaiseDeathEvent()
    {
        deaths++;
        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("Deaths", deaths);
        AnalyticsResult result = AnalyticsEvent.Custom("Deaths", customParams);
        Debug.Log("PlayerDeath " + result);
    }
    public void RaiseBossKillEvent()
    {
        defeatedBoss++;
        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("Boss Kills", defeatedBoss);
        AnalyticsResult result = AnalyticsEvent.Custom("Boss Kills", customParams);
        Debug.Log("BossKill " + result);
    }

    public void ToggleTimer(bool a)
    {
        timerStarted = a;
    }

    public void RaiseRunFinishedEvent()
    {
        runs++;
        Dictionary<string, object> playTimes = new Dictionary<string, object>();
        playTimes.Add(runs.ToString(), runTime);

        runTime = 0;
        ToggleTimer(false);
        AnalyticsResult result = AnalyticsEvent.Custom("RunTime Data", playTimes);
        Debug.Log("RunFinished " + result);
    }

    private void Update()
    {

        totalSessionTime += Time.deltaTime;
        if (timerStarted)
        {
            runTime += Time.deltaTime;
        }
    }

}
