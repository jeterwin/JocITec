using UnityEngine;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private SlowMotionController slowMo;
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private PlayerDetection detection;

    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [SerializeField] private float wallSlideSpeed = 1.5f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(18f, 20f);
    [SerializeField] private float wallJumpDuration = 0.2f;

    [SerializeField] private float grappleRange = 10f;
    [SerializeField] private float swingForce = 40f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer ropeRenderer;

    private Rigidbody2D rb;
    private DistanceJoint2D grappleJoint;
    private bool isGrappling;
    private bool canDash = true;

    public bool IsDashing { get; private set; }
    public bool IsWallSliding { get; private set; }
    public bool IsWallJumping { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        ropeRenderer.enabled = false;
    }

    private void Update()
    {
        IsWallSliding = (detection.IsWallLeft || detection.IsWallRight) && !detection.IsGrounded && rb.linearVelocity.y < 0;

        if (Input.GetKeyDown(KeyCode.LeftShift) && slowMo.CurrentSelection == "Dash" && canDash)
        {
            StartCoroutine(PerformDash());
        }

        if (Input.GetKeyDown(KeyCode.R) && slowMo.CurrentSelection == "Grapple")
        {
            HandleGrapple();
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (IsWallSliding)
            {
                StopCoroutine(nameof(PerformWallJump));
                StartCoroutine(PerformWallJump());
            }
            else if (!detection.IsGrounded && movement.CoyoteCounter <= 0f && movement.CanDoubleJump && slowMo.CurrentSelection == "Jump")
            {
                PerformDoubleJump();
            }
        }

        if (IsWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }

        if (isGrappling) ropeRenderer.SetPosition(0, transform.position);
    }

    private void FixedUpdate()
    {
        if (isGrappling) rb.AddForce(new Vector2(movement.HorizontalInput * swingForce, 0));
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        IsDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dir = movement.HorizontalInput != 0 ? movement.HorizontalInput : transform.localScale.x;
        rb.linearVelocity = new Vector2(dir * dashPower, 0f);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        IsDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator PerformWallJump()
    {
        IsWallJumping = true;
        float jumpDirection = detection.IsWallLeft ? 1 : -1;
        rb.linearVelocity = new Vector2(jumpDirection * wallJumpForce.x, wallJumpForce.y);
        yield return new WaitForSeconds(wallJumpDuration);
        IsWallJumping = false;
    }

    private void PerformDoubleJump()
    {
        movement.UseDoubleJump();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, movement.JumpForce);
    }

    private void HandleGrapple()
    {
        if (!isGrappling)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, grappleRange, grappleLayer);
            Collider2D bestTarget = null;
            float closestDist = float.MaxValue;

            foreach (var t in targets)
            {
                float d = Vector2.Distance(transform.position, t.transform.position);
                if (d < closestDist) { closestDist = d; bestTarget = t; }
            }

            if (bestTarget != null)
            {
                isGrappling = true;
                grappleJoint.enabled = true;
                grappleJoint.connectedAnchor = bestTarget.transform.position;
                grappleJoint.distance = Vector2.Distance(transform.position, bestTarget.transform.position);
                ropeRenderer.enabled = true;
                ropeRenderer.SetPosition(1, bestTarget.transform.position);
            }
        }
        else
        {
            isGrappling = false;
            grappleJoint.enabled = false;
            ropeRenderer.enabled = false;
        }
    }
}