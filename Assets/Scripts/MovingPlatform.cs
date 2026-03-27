using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 3f;

    [Header("Settings")]
    [SerializeField] private bool useEasing = true;
    [SerializeField] private bool usePause = true;
    [SerializeField] private float pauseDuration = 1f;

    private Vector3 currentStart;
    private Vector3 currentTarget;
    private float progress;
    private float waitTimer;
    private bool isWaiting;

    private void Start()
    {
        currentStart = pointA.position;
        currentTarget = pointB.position;
    }

    // Use FixedUpdate for anything involving physics or moving players
    private void FixedUpdate()
    {
        if (isWaiting)
        {
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= pauseDuration)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }

        float distance = Vector3.Distance(currentStart, currentTarget);
        if (distance > 0.01f)
        {
            progress += (speed / distance) * Time.fixedDeltaTime;
        }
        else
        {
            progress = 1f;
        }

        float movementT = useEasing ? Mathf.SmoothStep(0, 1, progress) : progress;

        // Move the platform
        transform.position = Vector3.Lerp(currentStart, currentTarget, movementT);

        if (progress >= 1f)
        {
            if (usePause) isWaiting = true;

            progress = 0f;
            Vector3 temp = currentStart;
            currentStart = currentTarget;
            currentTarget = temp;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}