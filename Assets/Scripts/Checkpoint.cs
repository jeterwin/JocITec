using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem checkpointVFX;
    [SerializeField] private int coinsReward = 1;
    private bool isActivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.CompareTag("Player"))
        {
            Activate();
        }
    }

    private void Activate()
    {
        isActivated = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateCheckpoint(transform.position);
        }

        AbilityCurrency.Instance.AddCoins(coinsReward);

        if (anim != null)
        {
            anim.Play("Flag");
        }

        if (checkpointVFX != null)
        {
            checkpointVFX.Play();
        }
    }
}