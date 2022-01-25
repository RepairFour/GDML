using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float degreesPerSecond = 45;

    public void Update()
    {
        transform.Rotate(transform.parent.right, degreesPerSecond * Time.deltaTime);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,0 , 0);
    }
}
