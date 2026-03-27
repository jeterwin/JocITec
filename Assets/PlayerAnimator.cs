using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;

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

    private void Update()
    {
        HandleFlipping();
        UpdateAnimationState();
    }

    private void HandleFlipping()
    {
        if (movement.IsWallSliding)
        {
            if (movement.IsWallLeft) transform.localScale = new Vector3(1, 1, 1);
            else if (movement.IsWallRight) transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement.IsWallJumping)
        {
            return;
        }
        else if (Mathf.Abs(movement.HorizontalInput) > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.HorizontalInput), 1, 1);
        }
    }

    private void UpdateAnimationState()
    {
        anim.SetFloat(YVelocity, rb.linearVelocity.y);
        anim.SetBool(IsGrounded, movement.IsGrounded);

        if (movement.IsDashing && !movement.IsWallJumping)
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

        if (movement.IsWallJumping || (_wasWallJumping && !movement.IsGrounded && rb.linearVelocity.y > -1f))
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

        if (!movement.IsGrounded)
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