using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }
    public bool IsWallLeft { get; private set; }
    public bool IsWallRight { get; private set; }
    public bool IsTouchingWall => IsWallLeft || IsWallRight;

    private void Update()
    {
        IsGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        IsWallLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, wallCheckDistance, groundLayer);
        IsWallRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, wallCheckDistance, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (groundCheck) Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        if (wallCheckLeft) Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * wallCheckDistance);
        if (wallCheckRight) Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * wallCheckDistance);
    }
}