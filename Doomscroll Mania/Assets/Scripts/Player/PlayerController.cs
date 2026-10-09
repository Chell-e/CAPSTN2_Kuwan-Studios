using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
        [Header("Sprite & Animator")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

        [Header("Player")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;

        [Header("Attack")]
    [SerializeField] private float damage = 1f;

        [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    private float inputX;
    private Vector2 lastFacingDirectionX = Vector2.right;

    // * DRIVER CODE
    // mainly Start() and Update()
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() { }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    private void GetInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        Move();

        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        if (Input.GetMouseButtonUp(0))
        {
            animator.ResetTrigger("Attack");
        }

        UpdatePlayerSprite(inputX);
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    private void Move()
    {
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void UpdatePlayerSprite(float inputX)
    {
        if (inputX != 0)
        {
            spriteRenderer.flipX = inputX < 0 ? true : false;
        }

        // animation
        if (inputX != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Health>()?.ChangeHealth(-damage);
    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS

}
