using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

#region enums
public enum TweenType
{
    Single,
    PingPong,
    Looping
}

public enum Ease
{
    none,
    easeInExpo,
    easeOutSine,
    easeInQuint,
    easeOutBounce,
    easeInBack
}

public enum TransformSection
{
    position,
    scale
}

public enum SetVectorTo
{
    None,
    LocalPosition,
    LocalScale
}

#endregion

public class TweenTransform : MonoBehaviour
{
	#region Variables
	public TransformSection affectedSection;
    public TweenType tweenType;
    public Ease ease;
    public float duration;
    public Vector3 startVector;
    public Vector3 endVector;
    public SetVectorTo SetStartVector;
    public bool loop = false;
    public UnityEvent OnTweenEnd;
    float halfDuration;
    private bool endCoroutines = false;

	#endregion

	#region Methods
	// Start is called before the first frame update
	void Start()
    {
        if (SetStartVector == SetVectorTo.LocalPosition)
        {
            startVector = transform.localPosition;
        }
        else if (SetStartVector == SetVectorTo.LocalScale)
        {
            startVector = transform.localScale;
        }
        halfDuration = duration / 2.0f;
    }

    public void Invoke()
    {

        switch (tweenType) //which tween type is it
        {

            case (TweenType.Looping):
                //continuous
                StartCoroutine(LoopingTween());
                break;
            case (TweenType.PingPong):
                StartCoroutine(PingPongTween());
                //backforth
                break;
            case (TweenType.Single):
                //one time
                StartCoroutine(SingleTween());
                break;
        }
    }

    public void Stop()
	{
        endCoroutines = true;
        StartCoroutine(ReturnTween());
    }
    ////////////TweensTypes/////////////////
    IEnumerator LoopingTween()
    {
        //get to end of tween
        //go back to start
        //repeat
        while (loop == true)
        {
            if (affectedSection == TransformSection.position)//affecting position vector
            {
                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    float t = elapsedTime / duration;
                    t = EaseT(t);
                    transform.localPosition = Vector3.LerpUnclamped(startVector, endVector, t);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                transform.localPosition = endVector;
                yield return null;
            }
            if (affectedSection == TransformSection.scale) // affecting scale
            {
                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    float t = elapsedTime / duration;
                    t = EaseT(t);
                    transform.localScale = Vector3.LerpUnclamped(startVector, endVector, t);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                transform.localScale = endVector;
                yield return null;
            }
        }

        if (loop != true)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (affectedSection == TransformSection.position)//affecting position vector
                {
                    float elapsedTime = 0;
                    while (elapsedTime < duration)
                    {
                        float t = elapsedTime / duration;
                        t = EaseT(t);
                        transform.localPosition = Vector3.LerpUnclamped(startVector, endVector, t);
                        elapsedTime += Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    transform.localPosition = endVector;
                    yield return null;
                }
                if (affectedSection == TransformSection.scale)
                {
                    float elapsedTime = 0;
                    while (elapsedTime < duration)
                    {
                        float t = elapsedTime / duration;
                        t = EaseT(t);
                        transform.localScale = Vector3.LerpUnclamped(startVector, endVector, t);
                        elapsedTime += Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    transform.localScale = endVector;
                    yield return null;
                }
            }

        }
        OnTweenEnd.Invoke();
    }

    IEnumerator PingPongTween()
    {
        //reach the end of the tween
        //Go from back to front
        //Then go back to end vector
        while (loop == true && !endCoroutines)
        {

            for (int i = 0; i < 2; ++i)
            {
                if (affectedSection == TransformSection.position) //affecting position vector
                {
                    if (i == 0)//first pass - reach the end
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localPosition = Vector3.LerpUnclamped(startVector, endVector, t);
                            elapsedTime += Time.deltaTime;
                            if (endCoroutines)
                            {
                                yield break;
                            }
                            yield return new WaitForEndOfFrame();
                        }
                        transform.localPosition = endVector;
                        yield return null;
                    }
                    else //second pass go back to start
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localPosition = Vector3.LerpUnclamped(endVector, startVector, t);
                            elapsedTime += Time.deltaTime;
                            if (endCoroutines)
                            {
                                yield break;
                            }
                            yield return new WaitForEndOfFrame();
                        }
                        transform.localPosition = startVector;
                        yield return null;
                    }

                }
                if (affectedSection == TransformSection.scale) //affecting scale vector
                {
                    if (i == 0)//first pass and third pass- reach the end
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration && !endCoroutines)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localScale = Vector3.LerpUnclamped(startVector, endVector, t);
                            elapsedTime += Time.deltaTime;

                            yield return new WaitForEndOfFrame();
                        }
                        transform.localScale = endVector;
                        yield return null;
                    }
                    else //second pass go back to start
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration && !endCoroutines)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localScale = Vector3.LerpUnclamped(endVector, startVector, t);
                            elapsedTime += Time.deltaTime;

                            yield return new WaitForEndOfFrame();
                        }
                        transform.localScale = startVector;
                        yield return null;
                    }
                }
            }
        }
        //Loop once
        if (loop != true)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (affectedSection == TransformSection.position) //affecting position vector
                {
                    if (i == 0 || i == 2)//first pass and third pass- reach the end
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration && !endCoroutines)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localPosition = Vector3.LerpUnclamped(startVector, endVector, t);
                            elapsedTime += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                        transform.localPosition = endVector;
                        yield return null;
                    }
                    else //second pass go back to start
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration && !endCoroutines)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localPosition = Vector3.LerpUnclamped(endVector, startVector, t);
                            elapsedTime += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                        transform.localPosition = startVector;
                        yield return null;
                    }

                }
                if (affectedSection == TransformSection.scale) //affecting scale vector
                {
                    if (i == 0 || i == 2)//first pass and third pass- reach the end
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration && !endCoroutines)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localScale = Vector3.LerpUnclamped(startVector, endVector, t);
                            elapsedTime += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                        transform.localScale = endVector;
                        yield return null;
                    }
                    else //second pass go back to start
                    {
                        float elapsedTime = 0;
                        while (elapsedTime < duration && !endCoroutines)
                        {
                            float t = elapsedTime / duration;
                            t = EaseT(t);
                            transform.localScale = Vector3.LerpUnclamped(endVector, startVector, t);
                            elapsedTime += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                        transform.localScale = startVector;
                        yield return null;
                    }
                }
            }
            OnTweenEnd.Invoke();

        }
    }
    IEnumerator SingleTween()
    {

        //Reach end of tween
        //stop
        if (affectedSection == TransformSection.position)//affecting position vector
        {
            float elapsedTime = 0;
            while (elapsedTime < duration && !endCoroutines)
            {
                float t = elapsedTime / duration;
                t = EaseT(t);
                transform.localPosition = Vector3.LerpUnclamped(startVector, endVector, t);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localPosition = endVector;
            yield return null;
        }
        if (affectedSection == TransformSection.scale) //affecting scale vector
        {
            float elapsedTime = 0;
            while (elapsedTime < duration && !endCoroutines)
            {
                float t = elapsedTime / duration;
                t = EaseT(t);
                transform.localScale = Vector3.LerpUnclamped(startVector, endVector, t);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = endVector;
            yield return null;
        }

        OnTweenEnd.Invoke();
    }
    IEnumerator ReturnTween()
    {
        //Reach end of tween
        //stop
        if (affectedSection == TransformSection.position)//affecting position vector
        {
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = EaseT(t);
                transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, startVector, t);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localPosition = startVector;
            yield return null;
        }
        if (affectedSection == TransformSection.scale) //affecting scale vector
        {
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = EaseT(t);
                transform.localScale = Vector3.LerpUnclamped(transform.localScale, startVector, t);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = startVector;
            yield return null;
        }
        endCoroutines = false;
        OnTweenEnd.Invoke();
    }

    ////////////Eases/////////////////

    float EaseT(float t)
    {
        switch (ease)
        {
            case (Ease.none):
                return t;
                break;
            case (Ease.easeInExpo):
                return easeInExpo(t);
                break;
            case (Ease.easeOutSine):
                return easeOutSine(t);
                break;
            case (Ease.easeInQuint):
                return easeInQuint(t);
                break;
            case (Ease.easeOutBounce):
                return easeOutBounce(t);
                break;
            case (Ease.easeInBack):
                return easeInBack(t);
                break;

        }
        return 0;
    }

    float easeInExpo(float t)
    {
        return Mathf.Pow(2, (10 * t - 10));
    }

    float easeOutSine(float t)
    {
        return Mathf.Sin((t * Mathf.PI) / 2);
    }

    float easeInQuint(float t)
    {
        return t * t * t * t * t;
    }

    float easeOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1 / d1)
        {
            return n1 * t * t;
        }
        else if (t < 2 / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5 / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }

    float easeInBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        return c3 * t * t * t - c1 * t * t;
    }
	#endregion
}

