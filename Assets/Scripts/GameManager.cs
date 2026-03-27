using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Animator deathAnimator;

    [Header("Settings")]
    [SerializeField] private float transitionTime = 0.5f;

    private Vector3 _currentCheckpoint;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (deathScreen != null) deathScreen.SetActive(false);
    }

    public void UpdateCheckpoint(Vector3 pos)
    {
        _currentCheckpoint = pos;
    }

    public void Respawn(GameObject player)
    {
        StartCoroutine(RespawnSequence(player));
    }

    private IEnumerator RespawnSequence(GameObject player)
    {
        deathScreen.SetActive(true);
        deathAnimator.Play("FadeIn");

        yield return new WaitForSeconds(transitionTime);

        player.transform.position = _currentCheckpoint;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        deathAnimator.Play("FadeOut");

        yield return new WaitForSeconds(transitionTime);

        deathScreen.SetActive(false);
    }
}