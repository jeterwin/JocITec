using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement Instance;

    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float acceleration = 90f;
    [SerializeField] private float deceleration = 80f;
    [SerializeField] private float airAcceleration = 100f;
    [SerializeField] private float airDeceleration = 30f;
    [SerializeField] private float jumpForce = 22f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [SerializeField] private float gravityScale = 4f;
    [SerializeField] private float fallMultiplier = 7f;
    [SerializeField] private float lowJumpMultiplier = 12f;
    [SerializeField] private float apexGravityMultiplier = 0.5f;

    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(15f, 22f);
    [SerializeField] private float wallJumpDuration = 0.15f;

    [SerializeField] private ParticleSystem walkParticles;
    [SerializeField] private ParticleSystem wallSlideParticles;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool canMove = true;

    private Rigidbody2D rb;
    private PlayerDetection detection;
    private PlayerAbilities abilities;
    private float horizontalInput;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool canDoubleJump;

    private ParticleSystem.EmissionModule walkEmission;
    private ParticleSystem.EmissionModule wallEmission;
    private ParticleSystem.MainModule walkMain;
    private ParticleSystem.MainModule wallMain;

    public float HorizontalInput => horizontalInput;

    public float CoyoteCounter => coyoteCounter;
    public bool IsDoubleJumping { get; private set; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool CanDoubleJump => canDoubleJump;
    public float JumpForce => jumpForce;
    public bool IsWallSliding { get; private set; }
    public bool IsWallJumping { get; private set; }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        detection = GetComponent<PlayerDetection>();
        abilities = GetComponent<PlayerAbilities>();

        if (walkParticles != null)
        {
            walkEmission = walkParticles.emission;
            walkMain = walkParticles.main;
        }
        if (wallSlideParticles != null)
        {
            wallEmission = wallSlideParticles.emission;
            wallMain = wallSlideParticles.main;
        }
    }

    private void Update()
    {
        if (!canMove) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        bool isTouchingWall = (detection.IsWallLeft || detection.IsWallRight);
        IsWallSliding = isTouchingWall && !detection.IsGrounded && rb.linearVelocity.y < 0;

        HandleParticles();
        AdjustGravity();

        if (detection.IsGrounded)
        {
            coyoteCounter = coyoteTime;
            canDoubleJump = true;
            IsDoubleJumping = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            if (coyoteCounter > 0f)
            {
                ApplyJump();
            }
            else if (IsWallSliding || isTouchingWall)
            {
                StopCoroutine(nameof(PerformWallJump));
                StartCoroutine(PerformWallJump());
            }
        }

        if (IsWallSliding && !IsWallJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (IsWallJumping || (abilities != null && abilities.IsDashing)) return;

        float targetSpeed = horizontalInput * maxSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;

        float accelRate;
        if (detection.IsGrounded)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? airAcceleration : airDeceleration;
        }

        rb.AddForce(speedDif * accelRate * Vector2.right, ForceMode2D.Force);
    }

    private void AdjustGravity()
    {
        if (IsWallSliding)
        {
            rb.gravityScale = 0;
            return;
        }

        if (Mathf.Abs(rb.linearVelocity.y) < 1.5f && !detection.IsGrounded)
        {
            rb.gravityScale = gravityScale * apexGravityMultiplier;
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void HandleParticles()
    {
        if (walkParticles != null)
        {
            bool isWalking = detection.IsGrounded && Mathf.Abs(horizontalInput) > 0.1f;
            walkEmission.enabled = isWalking;

            if (isWalking)
            {
                Vector2 rayStart = (Vector2)transform.position;
                RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 1.5f, groundLayer);

                if (hit.collider != null)
                {
                    walkMain.startColor = GetColorFromHit(hit);
                }
            }
        }

        if (wallSlideParticles != null)
        {
            wallEmission.enabled = IsWallSliding;

            if (IsWallSliding)
            {
                Vector2 rayDir = detection.IsWallLeft ? Vector2.left : Vector2.right;
                Vector2 rayStart = (Vector2)transform.position + (rayDir * 0.2f);
                RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDir, 1.5f, groundLayer);

                if (hit.collider != null)
                {
                    wallMain.startColor = GetColorFromHit(hit);
                }

                float rotY = detection.IsWallLeft ? 90f : -90f;
                wallSlideParticles.transform.rotation = Quaternion.Euler(0, rotY, 0);
            }
        }
    }

    private Color GetColorFromHit(RaycastHit2D hit)
    {
        if (hit.collider.TryGetComponent(out SpriteRenderer sr)) return sr.color;
        if (hit.collider.TryGetComponent(out Tilemap tm)) return tm.color;
        return Color.white;
    }

    public void ApplyJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
    }

    private IEnumerator PerformWallJump()
    {
        IsWallJumping = true;
        jumpBufferCounter = 0f;
        float jumpDirection = detection.IsWallLeft ? 1 : -1;
        rb.linearVelocity = new Vector2(jumpDirection * wallJumpForce.x, wallJumpForce.y);
        yield return new WaitForSeconds(wallJumpDuration);
        IsWallJumping = false;
    }

    public void UseDoubleJump()
    {
        canDoubleJump = false;
        IsDoubleJumping = true;
        jumpBufferCounter = 0f;
    }
}