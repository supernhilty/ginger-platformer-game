using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 25f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deadkick = new Vector2(20f,20f);
    Vector2 moveInput;
    Rigidbody2D rb2d;
    Animator animator;    
    CapsuleCollider2D capsuleCollider2D;    
    float gravityScaleAtStart;
    bool isAlive = true;
    

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = rb2d.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;
        Run();
        FlipSprite();
        ClimbLadder();      
        Die();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {            
            Invoke("StartAgain", 0.5f);
        }
    }
       void Die()
    {
        if (capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            rb2d.velocity = deadkick;
            StartAgain();
        }
    }
    void StartAgain()
    {
        SceneManager.LoadScene(0);
    }

    void ClimbLadder()
    {
        if (!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb2d.gravityScale = gravityScaleAtStart;
            animator.SetBool("isClimbing", false);
            return;
        }
        bool playerHasVerticalSpeed = Mathf.Abs(rb2d.velocity.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
        rb2d.gravityScale = 0f;
        Vector2 climbVectocity = new Vector2(rb2d.velocity.x, moveInput.y * climbSpeed);
        rb2d.velocity = climbVectocity;
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)       
            transform.localScale = new Vector2(Mathf.Sign(rb2d.velocity.x), 1f);        
    }

    void Run()
    {      
        Vector2 playerVectocity = new Vector2(moveInput.x  * runSpeed, rb2d.velocity.y);
        rb2d.velocity = playerVectocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);

    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
            return;
        moveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if (!isAlive)
            return;
        if (!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;
        if (value.isPressed)
        {
            rb2d.velocity += new Vector2(0f, jumpSpeed);
        }
    }
}
