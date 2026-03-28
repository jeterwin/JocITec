using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System.Collections.Generic;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private SlowMotionController slowMo;
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private PlayerDetection detection;
    [SerializeField] private AbilityCurrency currency;

    [SerializeField] private string jumpAbilityName = "Jump";
    [SerializeField] private string grappleAbilityName = "Grapple";
    [SerializeField] private string dashAbilityName = "Dash";

    [SerializeField] private KeyCode dashKeyCode = KeyCode.LeftShift;
    [SerializeField] private KeyCode grappleKeyCode = KeyCode.LeftShift;

    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [SerializeField] private float grappleRange = 10f;
    [SerializeField] private float swingForce = 40f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer ropeRenderer;

    [SerializeField] private CinemachineImpulseSource dashImpulse;

    private Rigidbody2D rb;
    private DistanceJoint2D grappleJoint;
    private bool isGrappling;
    private bool canDash = true;
    private List<string> unlockedAbilities = new();

    private string currentSelection = "None";
    private string pendingSelection = "None";

    public string CurrentSelection => currentSelection;
    public bool IsDashing { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        ropeRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            ConfirmSelection();
        }

        if (Input.GetKeyDown(dashKeyCode) && currentSelection == dashAbilityName && canDash)
        {
            if (currency.TrySpend(1)) StartCoroutine(PerformDash());
        }

        if (Input.GetKeyDown(grappleKeyCode) && currentSelection == grappleAbilityName)
        {
            HandleGrapple();
        }

        if (Input.GetButtonDown("Jump"))
        {
            bool isMidAir = !detection.IsGrounded && !movement.IsWallSliding && movement.CoyoteCounter <= 0f;

            if (isMidAir && movement.CanDoubleJump && currentSelection == jumpAbilityName)
            {
                if (currency.TrySpend(1)) PerformDoubleJump();
            }
        }

        if (isGrappling) ropeRenderer.SetPosition(0, transform.position);
    }

    private void FixedUpdate()
    {
        if (isGrappling) rb.AddForce(new Vector2(movement.HorizontalInput * swingForce, 0));
    }

    public void SetPendingSelection(string name)
    {
        if (unlockedAbilities.Contains(name))
        {
            pendingSelection = name;
        }
    }

    private void ConfirmSelection()
    {
        if (pendingSelection != currentSelection)
        {
            currentSelection = pendingSelection;
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        IsDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dir = movement.HorizontalInput != 0 ? movement.HorizontalInput : transform.localScale.x;
        rb.linearVelocity = new Vector2(dir * dashPower, 0f);
        if (dashImpulse != null) dashImpulse.GenerateImpulse(new Vector3(dir, 0, 0));
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        IsDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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

            if (bestTarget != null && currency.TrySpend(1))
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

    public void UnlockAbility(string name)
    {
        if (!unlockedAbilities.Contains(name))
        {
            unlockedAbilities.Add(name);
        }
    }
}