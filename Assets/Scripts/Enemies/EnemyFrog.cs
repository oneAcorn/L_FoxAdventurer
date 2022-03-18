using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrog : MonoBehaviour
{
    public Transform left, right;
    public float speed, jumpForce;
    [Space] public LayerMask ground;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    private Animator anim;
    private float leftX, rightX;
    private bool isGround;
    private bool isFaceRight = false;
    private bool isDeath = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        // transform.DetachChildren();
        leftX = left.position.x;
        rightX = right.position.x;
        Destroy(left.gameObject);
        Destroy(right.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        SwitchAnim();
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    private void Move()
    {
        if (!isGround || isDeath)
            return;
        if (transform.position.x > rightX)
        {
            renderer.flipX = false;
            // isFaceRight = false;
        }
        else if (transform.position.x < leftX)
        {
            renderer.flipX = true;
            // isFaceRight = true;
        }

        if (renderer.flipX)
        {
            rb.velocity = new Vector2(speed, jumpForce);
        }
        else
        {
            rb.velocity = new Vector2(-speed, jumpForce);
        }
    }

    private void SwitchAnim()
    {
        if (!isGround && rb.velocity.y > 0)
        {
            anim.SetBool("Jumpping", true);
        }
        else if (!isGround && rb.velocity.y <= 0)
        {
            anim.SetBool("Jumpping", false);
            anim.SetBool("Falling", true);
        }
        else if (isGround)
        {
            anim.SetBool("Jumpping", false);
            anim.SetBool("Falling", false);
        }
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    private void PrepareDeath()
    {
        isDeath = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
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