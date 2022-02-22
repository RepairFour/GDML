using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float timer = 0;
    float duration;
    public Timer(float _duration)
	{
        duration = _duration;
	}

    public void UpdateTimer()
	{
        timer += Time.deltaTime;
    }

    public bool IsFinished()
	{
        return timer > duration ? true : false;
	}

    public void ResetTimer()
	{
        timer = 0;
    }

    public void ChangeDuration(float newDuration)
	{
        duration = newDuration;
	}
}
