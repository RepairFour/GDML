using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer> lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
