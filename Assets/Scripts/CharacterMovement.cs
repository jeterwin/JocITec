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
    private bool isDoubleJumping;

    [Header("Dash")]
    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Wall Interaction")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.4f;
    [SerializeField] private float wallSlideSpeed = 1.5f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(18f, 20f);
    [SerializeField] private float wallJumpDuration = 0.2f;
    private bool isWallSliding;
    private bool isWallLeft;
    private bool isWallRight;
    private bool isWallJumping;

    [Header("Grappling")]
    [SerializeField] private float grappleRange = 10f;
    [SerializeField] private float swingForce = 40f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer ropeRenderer;
    private DistanceJoint2D grappleJoint;
    private bool isGrappling;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Color gizmoColor = Color.cyan;

    [SerializeField] private Rigidbody2D rb;

    private float horizontalInput;
    private float coyoteCounter;
    private bool isGrounded;

    public float HorizontalInput => horizontalInput;
    public bool IsGrounded => isGrounded;
    public bool IsWallSliding => isWallSliding;
    public bool IsDashing => isDashing;
    public bool IsWallJumping => isWallJumping;
    public bool IsDoubleJumping => isDoubleJumping;
    public bool CanDoubleJump => canDoubleJump;
    public bool IsWallLeft => isWallLeft;
    public bool IsWallRight => isWallRight;

    private void Awake()
    {
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        ropeRenderer.enabled = false;
    }

    private void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        isWallLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, wallCheckDistance, groundLayer);
        isWallRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, wallCheckDistance, groundLayer);

        isWallSliding = (isWallLeft || isWallRight) && !isGrounded;

        if (isGrounded || isWallSliding)
        {
            coyoteCounter = coyoteTime;
            canDoubleJump = true;
            isDoubleJumping = false;
            isWallJumping = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isGrappling) StartGrapple();
            else StopGrapple();
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrappling)
            {
                StopGrapple();
                Jump();
            }
            else if (coyoteCounter > 0f && isGrounded)
            {
                Jump();
            }
            else if (isWallSliding)
            {
                WallJump();
            }
            else if (canDoubleJump)
            {
                isDoubleJumping = true;
                Jump();
                canDoubleJump = false;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isGrappling)
        {
            StartCoroutine(Dash());
        }

        if (isGrappling)
        {
            ropeRenderer.SetPosition(0, transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        if (isGrappling)
        {
            rb.AddForce(new Vector2(horizontalInput * swingForce, 0));
            return;
        }

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }

        float targetSpeed = horizontalInput * maxSpeed;
        if (isWallLeft && targetSpeed < 0) targetSpeed = 0;
        if (isWallRight && targetSpeed > 0) targetSpeed = 0;

        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        rb.AddForce(speedDif * accelRate * Vector2.right, ForceMode2D.Force);
    }

    private void StartGrapple()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, grappleRange, grappleLayer);
        Collider2D bestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, groundLayer | grappleLayer);

            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & grappleLayer) != 0)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = target;
                }
            }
        }

        if (bestTarget != null)
        {
            isGrappling = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            grappleJoint.enabled = true;
            grappleJoint.enableCollision = true;
            grappleJoint.autoConfigureDistance = false;
            grappleJoint.connectedAnchor = bestTarget.transform.position;
            grappleJoint.distance = closestDistance;
            grappleJoint.maxDistanceOnly = true;

            ropeRenderer.enabled = true;
            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, bestTarget.transform.position);
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        grappleJoint.enabled = false;
        ropeRenderer.enabled = false;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;
    }

    private void WallJump()
    {
        isWallSliding = false;
        isWallJumping = true;
        float jumpDirection = isWallLeft ? 1 : -1;
        transform.localScale = new Vector3(jumpDirection, 1, 1);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(jumpDirection * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
        StartCoroutine(WallJumpLock());
    }

    private IEnumerator WallJumpLock()
    {
        isDashing = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isDashing = false;
        isWallJumping = false;
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
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
        if (groundCheck) Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        if (wallCheckLeft) Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * wallCheckDistance);
        if (wallCheckRight) Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * wallCheckDistance);
    }
}