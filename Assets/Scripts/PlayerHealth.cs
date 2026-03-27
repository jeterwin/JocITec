using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void Die()
    {
        GameManager.Instance.Respawn(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            Die();
        }
    }
}