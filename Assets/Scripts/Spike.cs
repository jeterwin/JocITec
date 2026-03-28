using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision);

        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.Die();
        }
    }
}