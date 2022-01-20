using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    public float startAlpha = 1f;
    public float fadeTime;
    float fadeTimer = 0;
    public Image image;

    public IEnumerator fadeImage()
    {
        var temp = image.color;
        temp.a = startAlpha;
        image.color = temp;
        
        while(fadeTimer < fadeTime)
        {
            fadeTimer += Time.deltaTime;
            temp.a = Mathf.Lerp(startAlpha, 0, fadeTimer / fadeTime);
            image.color = temp;
            yield return null;
        }
        fadeTimer = 0;
        StopCoroutine(fadeImage());
       
    }

}
