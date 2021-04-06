using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelEventManager : MonoBehaviour
{
    public enum LevelPlayState { InProgress, Won, Lost, Quit};

    private Scene thisScene;
    private LevelPlayState state = LevelPlayState.InProgress;
    private int secondsElasped = 0;
    private int deaths = 0;

    private void Awake()
    {
        thisScene = SceneManager.GetActiveScene();

        AnalyticsEvent.LevelStart(thisScene.name, thisScene.buildIndex);
    }
}
