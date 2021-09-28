using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    // Start is called before the first frame update

    public float DestroyAfter;
    void Start()
    {
        Destroy(gameObject, DestroyAfter);
    }

}
