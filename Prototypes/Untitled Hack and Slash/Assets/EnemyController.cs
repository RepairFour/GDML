using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    Vector3 direction;

    float moveTimer = 0;
    float speed = 5;
    // Start is called before the first frame update
    private void Awake()
    {
        direction = transform.forward;
        moveTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        moveTimer += Time.deltaTime;

        if (moveTimer >= 5f)
        {
            moveTimer = 0;
            direction = direction * -1;
        }
    }
}
