using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float deathJumpForce = 12f;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private float waitBeforeRespawn = 0.5f;

    [SerializeField] private ParticleSystem deathSFX;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private Animator fadeAnimator;

    [SerializeField] private CharacterMovement movement;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;

    private bool isDead;

    public void Die()
    {
        if (isDead) return;
        StartCoroutine(DeathSequence());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Die();
        }
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;
        movement.CanMove = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * deathJumpForce, ForceMode2D.Impulse);

        deathSFX.Play();
        if (deathSource != null) deathSource.Play();

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(blinkDuration);
            sr.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }

        yield return new WaitForSeconds(waitBeforeRespawn);

        GameManager.Instance.Respawn(gameObject);

        sr.color = Color.white;
        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            Die();
        }
    }
}