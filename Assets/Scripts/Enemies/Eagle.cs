using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    public Transform top, bottom;
    public float speed;

    private Rigidbody2D rb;
    private float topY, bottomY;

    private bool isUp = true;
    private bool isDeath = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        topY = top.position.y;
        bottomY = bottom.position.y;
        Destroy(top.gameObject);
        Destroy(bottom.gameObject);
    }

    private void FixedUpdate()
    {
        if(isDeath)
            return;
        // Debug.Log($"fly {rb.velocity.y},{topY},{isUp}");
        if (isUp)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
            if (transform.position.y > topY)
                isUp = false;
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -speed);
            if (transform.position.y < bottomY)
                isUp = true;
        }
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    private void PrepareDeath()
    {
        isDeath = true;
        rb.velocity=Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    private void Death()
    {
        Destroy(gameObject);
    }
}