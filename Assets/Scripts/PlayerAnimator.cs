using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerDetection detection;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerAbilities abilities;
    [SerializeField] private CharacterMovement movement;

    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;

    private Vector3 originalScale;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
    private static readonly int WallSlide = Animator.StringToHash("WallSlide");
    private static readonly int WallJump = Animator.StringToHash("WallJump");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    private bool _wasWallJumping;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        HandleFlipping();
        UpdateAnimationState();
    }

    private void HandleFlipping()
    {
        if (movement.IsWallSliding)
        {
            if (detection.IsWallLeft) transform.localScale = originalScale;
            else if (detection.IsWallRight) transform.localScale = 
                    new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else if (movement.IsWallJumping)
        {
            return;
        }
        else if (Mathf.Abs(movement.HorizontalInput) > 0.1f)
        {
            float direction = Mathf.Sign(movement.HorizontalInput);
            transform.localScale = new Vector3(direction * originalScale.x, originalScale.y, originalScale.z);
        }
    }

    private void UpdateAnimationState()
    {
        anim.SetFloat(YVelocity, rb.linearVelocity.y);
        anim.SetBool(IsGrounded, detection.IsGrounded);

        if (abilities.IsDashing && !movement.IsWallJumping)
        {
            _wasWallJumping = false;
            PlayAnimation(Dash);
            return;
        }

        if (movement.IsDoubleJumping)
        {
            _wasWallJumping = false;
            PlayAnimation(DoubleJump);
            return;
        }

        if (movement.IsWallJumping || (_wasWallJumping && !detection.IsGrounded && rb.linearVelocity.y > -1f))
        {
            _wasWallJumping = true;
            PlayAnimation(WallJump);
            return;
        }

        _wasWallJumping = false;

        if (movement.IsWallSliding)
        {
            PlayAnimation(WallSlide);
            return;
        }

        if (!detection.IsGrounded)
        {
            PlayAnimation(Jump);
        }
        else
        {
            if (Mathf.Abs(movement.HorizontalInput) > 0.1f)
                PlayAnimation(Walk);
            else
                PlayAnimation(Idle);
        }
    }

    private void PlayAnimation(int stateHash)
    {
        anim.CrossFade(stateHash, 0, 0);
    }
}