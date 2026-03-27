using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float coyoteTime = 0.15f;

    private Rigidbody2D rb;
    private PlayerDetection detection;
    private PlayerAbilities abilities;
    private float horizontalInput;
    private float coyoteCounter;
    private bool canDoubleJump;

    public float HorizontalInput => horizontalInput;
    public bool IsDoubleJumping { get; private set; }
    public bool CanDoubleJump => canDoubleJump;
    public float JumpForce => jumpForce;
    public float CoyoteCounter => coyoteCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        detection = GetComponent<PlayerDetection>();
        abilities = GetComponent<PlayerAbilities>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

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
            if (coyoteCounter > 0f && !abilities.IsWallSliding)
            {
                ApplyJump();
            }
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void FixedUpdate()
    {
        if (abilities != null && abilities.IsWallJumping) return;

        float targetSpeed = horizontalInput * maxSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        rb.AddForce(speedDif * accelRate * Vector2.right, ForceMode2D.Force);
    }

    public void ApplyJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;
    }

    public void UseDoubleJump()
    {
        canDoubleJump = false;
        IsDoubleJumping = true;
    }
}