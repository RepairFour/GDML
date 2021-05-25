using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenSequence : MonoBehaviour
{
    public List<TweenTransform> tweens;

    public void Invoke()
    {
        foreach(var tween in tweens)
        {
            tween.Invoke();
        }
    }
}
