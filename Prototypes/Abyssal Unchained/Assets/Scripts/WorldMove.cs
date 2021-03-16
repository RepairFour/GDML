using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMove : MonoBehaviour
{
    Transform worldTransform;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        worldTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        worldTransform.position -= new Vector3(speed * Time.deltaTime,0,0);
    }
}
