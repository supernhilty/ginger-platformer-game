using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    Vector2 moveInput;
    Rigidbody2D rb2d;
    Animator animator;
    [SerializeField] float jumpSpeed = 25f;
    CapsuleCollider2D capsuleCollider2D;
    [SerializeField] float climbSpeed = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void ClimbLadder()
    {
        if (!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
            return;
        Vector2 climbVectocity = new Vector2(rb2d.velocity.y, moveInput.x * climbSpeed);
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
        
        moveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if (!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;
        if (value.isPressed)
        {
            rb2d.velocity += new Vector2(0f, jumpSpeed);
        }
    }
}
