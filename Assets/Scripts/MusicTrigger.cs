using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip musicToPlay;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && musicToPlay != null)
        {
            AudioManager.Instance.ChangeBackgroundMusic(musicToPlay);
        }
    }
}