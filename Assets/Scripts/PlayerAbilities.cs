using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private SlowMotionController slowMo;
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private PlayerDetection detection;
    [SerializeField] private AbilityCurrency currency;

    [SerializeField] private string jumpAbilityName = "Jump";
    [SerializeField] private string grappleAbilityName = "Grapple";
    [SerializeField] private string dashAbilityName = "Dash";

    [SerializeField] private Button jumpButton;
    [SerializeField] private Button grappleButton;
    [SerializeField] private Button dashButton;

    [SerializeField] private KeyCode dashKeyCode = KeyCode.LeftShift;
    [SerializeField] private KeyCode grappleKeyCode = KeyCode.LeftShift;

    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [SerializeField] private float grappleRange = 10f;
    [SerializeField] private float swingForce = 40f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer ropeRenderer;
    [SerializeField] private GameObject grappleIndicator;

    [SerializeField] private CinemachineImpulseSource dashImpulse;

    private Rigidbody2D rb;
    private DistanceJoint2D grappleJoint;
    private bool isGrappling;
    private bool canDash = true;
    private List<string> unlockedAbilities = new();

    private string currentSelection = "None";
    private string pendingSelection = "None";

    private List<Collider2D> targetsInRange = new();

    public string CurrentSelection => currentSelection;
    public bool IsDashing { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        ropeRenderer.enabled = false;

        if (grappleIndicator != null) grappleIndicator.SetActive(false);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        if (jumpButton != null) jumpButton.interactable = false;
        if (grappleButton != null) grappleButton.interactable = false;
        if (dashButton != null) dashButton.interactable = false;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Grapple"))
        {
            if (!targetsInRange.Contains(other))
            {
                targetsInRange.Add(other);
                grappleIndicator.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grapple"))
        {
            targetsInRange.Remove(other);
            grappleIndicator.SetActive(false);
        }
    }

    private Collider2D GetBestGrappleTarget()
    {
        targetsInRange.RemoveAll(item => item == null || !item.gameObject.activeInHierarchy);

        Collider2D bestTarget = null;
        float closestDist = float.MaxValue;

        foreach (var t in targetsInRange)
        {
            float d = Vector2.Distance(transform.position, t.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                bestTarget = t;
            }
        }

        return bestTarget;
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

        AudioManager.Instance.DashPlay();

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
        AudioManager.Instance.DoubleJumpPlay();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, movement.JumpForce);
    }

    private void HandleGrapple()
    {
        if (!isGrappling)
        {
            Collider2D bestTarget = GetBestGrappleTarget();

            if (bestTarget != null && currency.TrySpend(1))
            {
                AudioManager.Instance.GrapplePlay();

                isGrappling = true;
                grappleJoint.enabled = true;
                grappleJoint.connectedAnchor = bestTarget.transform.position;
                grappleJoint.distance = Vector2.Distance(transform.position, bestTarget.transform.position);
                ropeRenderer.enabled = true;
                ropeRenderer.SetPosition(1, bestTarget.transform.position);

                if (grappleIndicator != null) grappleIndicator.SetActive(false);
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
            UpdateButtonState(name, true);
        }
    }

    private void UpdateButtonState(string name, bool state)
    {
        if (name == jumpAbilityName && jumpButton != null) jumpButton.interactable = state;
        else if (name == grappleAbilityName && grappleButton != null) grappleButton.interactable = state;
        else if (name == dashAbilityName && dashButton != null) dashButton.interactable = state;
    }
}