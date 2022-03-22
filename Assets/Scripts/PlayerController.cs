using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    [Tooltip("可以关闭的碰撞器")] public Collider2D disableColl;
    public Transform cellingCheck;
    public Transform groundCheck;
    public LayerMask ground;
    public Text cherryNumber;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    private bool isGround, isJump;
    private bool jumpPressed;
    private int jumpCount;

    private int cherryCount;
    public bool isHurt;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }

        CrouchInput();
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        if (!isHurt)
        {
            GroundMove();
            Jump();
        }

        SwitchAnim();
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        //收集物品
        if (other.CompareTag("Collection"))
        {
            Destroy(other.gameObject);
            cherryCount++;
            cherryNumber.text = cherryCount.ToString();
        }

        Debug.Log($"trigger:{other.gameObject.tag}");
        if (other.CompareTag("DeadArea")) //摔死了
        {
            StartCoroutine(Die(true));
        }
    }

    /// <summary>
    /// 消灭敌人
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (anim.GetBool("Falling"))
            {
                // Debug.Log($"击杀 {other.gameObject.name},{other.gameObject.GetComponent<Animator>()}");
                //击杀
                other.gameObject.GetComponent<Animator>().SetTrigger("Death");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
                anim.SetBool("Jumpping", true);
            }
            else
            {
                //挨打
                float retreatDistance = transform.position.x < other.gameObject.transform.position.x ? -5 : 5;
                isHurt = true;
                //被击退
                rb.velocity = new Vector2(retreatDistance, rb.velocity.y);
            }
        }
    }

    #region 移动,跳跃

    private void GroundMove()
    {
        //这方法返回值只有3个 -1,0,1
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        //在FixedUpdate里得乘以Time.fixedDeltaTime
        rb.velocity = new Vector2(horizontalMove * speed * Time.fixedDeltaTime, rb.velocity.y);

        if (horizontalMove != 0)
        {
            renderer.flipX = horizontalMove < 0;
        }
    }

    private void Jump()
    {
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }

        if (jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            jumpCount--;
            jumpPressed = false;
            SoundManager.instance.PlayJump1();
        }
        else if (jumpPressed && jumpCount > 0 && !isGround)
        {
            //多段跳
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.4f * Time.fixedDeltaTime);
            jumpCount--;
            jumpPressed = false;
            SoundManager.instance.PlayJump2();
        }
    }

    private void CrouchInput()
    {
        if (!Physics2D.OverlapCircle(cellingCheck.position, 0.2f, ground)) //头顶没有阻挡物
        {
            if (Input.GetButton("Crouch"))
            {
                anim.SetBool("Crouching", true);
                disableColl.enabled = false;
            }
            else
            {
                anim.SetBool("Crouching", false);
                disableColl.enabled = true;
            }
        }
    }

    private void SwitchAnim()
    {
        // Debug.Log($"anim {isGround},{rb.velocity.y}");
        anim.SetFloat("Running", Mathf.Abs(rb.velocity.x));
        if (isGround)
        {
            anim.SetBool("Jumpping", false);
            anim.SetBool("Falling", false);
        }
        else if (!isGround && rb.velocity.y > 0)
        {
            //跳起上升中
            anim.SetBool("Jumpping", true);
            anim.SetBool("Falling", false);
        }
        else if (rb.velocity.y < 0)
        {
            // Debug.Log("Falling");
            anim.SetBool("Jumpping", false);
            anim.SetBool("Falling", true);
        }

        if (isHurt)
        {
            anim.SetBool("Hurt", true);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                //击退效果结束
                isHurt = false;
                anim.SetBool("Hurt", false);
            }
        }
    }

    #endregion

    private IEnumerator Die(bool isFall)
    {
        if (isFall)
        {
            SoundManager.instance.PlayFall();
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}