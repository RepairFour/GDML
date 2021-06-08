using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTremor : MonoBehaviour
{
    int direction;
    Vector2 tremorPower;
    Rigidbody2D rb;
    bool moving = false;

    [SerializeField] GameObject tremorExplosion;

    public int setDirection {set => direction = value; }
    public Vector2 setTremorPower {set => tremorPower = value; }
    public bool setMoving {set => moving = value; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(rb.velocity.x < 0.01 && rb.velocity.x > -0.01)
        {
            if (moving)
            {
                tremorPower *= direction;
                rb.velocity = tremorPower;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(rb.velocity.x < 0 && collision.CompareTag("TremorExplodeLeft"))
        {
            Instantiate(tremorExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if(rb.velocity.x > 0 && collision.CompareTag("TremorExplodeRight"))
        {
            Instantiate(tremorExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
