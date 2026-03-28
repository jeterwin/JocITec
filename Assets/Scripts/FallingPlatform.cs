using UnityEngine;
using System.Collections;

public class OneWayFallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f;
    public float respawnDelay = 2.5f;
    public float shakeMagnitude = 0.06f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Quaternion startRot;
    private bool isFalling = false;
    private Collider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    StartCoroutine(FallSequence());
                    break;
                }
            }
        }
    }

    private IEnumerator FallSequence()
    {
        isFalling = true;

        float timer = 0;
        while (timer < fallDelay)
        {
            transform.position = startPos + (Vector2)Random.insideUnitCircle * shakeMagnitude;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(respawnDelay);

        col.enabled = false;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = startPos;
        transform.rotation = startRot;

        yield return new WaitForSeconds(1.0f);

        col.enabled = true;
        isFalling = false;
    }
}