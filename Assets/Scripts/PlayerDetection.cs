using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
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
        IsGrounded = CheckGround();
        IsWallLeft = CheckWall(wallCheckLeft.position, Vector2.left);
        IsWallRight = CheckWall(wallCheckRight.position, Vector2.right);
    }

    private bool CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject || col.isTrigger) continue;

            // Platforms ARE counted as ground so you can stand/jump on them
            if (col.CompareTag("Platform") || (groundLayer.value & (1 << col.gameObject.layer)) != 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckWall(Vector2 pos, Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, direction, wallCheckDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject || hit.collider.isTrigger) continue;

            // EXPLICITLY SKIP: If it's a platform, ignore it for wall mechanics
            if (hit.collider.CompareTag("Platform")) continue;

            // Otherwise, check if it's on the ground layer
            if ((groundLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (groundCheck) Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        if (wallCheckLeft) Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * wallCheckDistance);
        if (wallCheckRight) Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * wallCheckDistance);
    }
}