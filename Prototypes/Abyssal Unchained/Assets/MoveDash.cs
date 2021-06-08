using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDash : MonoBehaviour
{
    float speed;

    Vector3 direction;
    Rigidbody2D rb;

    bool objectFinished = false;

    public bool isObjectFinished { get => objectFinished; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetSpeed(float s)
    {
        speed = s;
    }

    public void SetDirection(Vector3 d)
    {
        direction = d;
    }

    public void SetVelocity()
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("UpperBound") ||
            collision.gameObject.CompareTag("RightProjectileBounds") ||
            collision.gameObject.CompareTag("LeftProjectileBounds"))
        {
            objectFinished = true;
        }
    }
}
