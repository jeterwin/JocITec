using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 60f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float coyoteTime = 0.15f;

    private bool canDoubleJump;

    [Header("Dash")]
    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;
    private bool isDashing;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Color gizmoColor = Color.red;

    [SerializeField] private Rigidbody2D rb;

    private float horizontalInput;
    private float coyoteCounter;
    private bool isGrounded;

    private void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canDoubleJump = true;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (coyoteCounter > 0f)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        float targetSpeed = horizontalInput * maxSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = horizontalInput != 0 ? horizontalInput : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDirection * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
    }
}