using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameTrigger : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI rawTimeText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    [Header("Cinematic Settings")]
    [SerializeField] private Transform destination1;
    [SerializeField] private Transform destination2;
    [SerializeField] private float arcHeight = 5f;
    [SerializeField] private float lerpTime = 1.5f;
    [SerializeField] private float travelSpeed = 10f;
    [SerializeField] private GameObject[] firstActivationGroup;
    [SerializeField] private GameObject[] secondActivationGroup;

    [Header("Transition")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Animator deathAnimator;
    [SerializeField] private float transitionTime = 0.5f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered || !collision.CompareTag("Player")) return;
        triggered = true;

        StartCoroutine(PlayEndSequence(collision.transform));
    }

    private IEnumerator PlayEndSequence(Transform player)
    {
        CharacterMovement.Instance.CanMove = false;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        Vector3 startPos = player.position;
        float elapsed = 0;

        while (elapsed < lerpTime)
        {
            float t = elapsed / lerpTime;
            Vector3 basePos = Vector3.Lerp(startPos, destination1.position, t);
            float height = arcHeight * 4 * (t - t * t);
            player.position = basePos + Vector3.up * height;

            elapsed += Time.deltaTime;
            yield return null;
        }
        player.position = destination1.position;

        foreach (GameObject obj in firstActivationGroup)
        {
            if (obj != null) obj.SetActive(true);
        }

        while (Vector3.Distance(player.position, destination2.position) > 0.01f)
        {
            player.position = Vector3.MoveTowards(player.position, destination2.position, travelSpeed * Time.deltaTime);
            yield return null;
        }
        player.position = destination2.position;

        foreach (GameObject obj in secondActivationGroup)
        {
            if (obj != null) obj.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        ShowEndScreen();

        yield return new WaitForSeconds(7f);
        GoToMainMenu();
    }

    private void ShowEndScreen()
    {
        GameTimer.Instance.StopTimer();

        float raw = GameTimer.Instance.ElapsedTime;
        int coins = AbilityCurrency.Instance.CurrentCoins;
        float bonus = coins * GameTimer.Instance.SecondsPerCoin;
        float final = GameTimer.Instance.GetFinalTime();

        // High Score Logic: Save if the new time is lower (faster)
        float currentBest = PlayerPrefs.GetFloat("BestTime", 999999f);
        if (final < currentBest)
        {
            PlayerPrefs.SetFloat("BestTime", final);
            PlayerPrefs.Save();
        }

        endPanel.SetActive(true);

        if (rawTimeText != null) rawTimeText.text = $"Total Time: {GameTimer.FormatTime(raw)}";
        if (coinsText != null) coinsText.text = $"Remaining Coins: {coins}";
        if (bonusText != null) bonusText.text = $"Bonus: -{GameTimer.FormatTime(bonus)}";
        if (finalTimeText != null) finalTimeText.text = $"FINAL TIME: {GameTimer.FormatTime(final)}";
    }

    public void GoToMainMenu()
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        deathScreen.SetActive(true);

        deathAnimator.Play("FadeIn");

        yield return new WaitForSeconds(transitionTime);

   
        endPanel.SetActive(false);
        SceneManager.LoadScene("Main Menu");
    }
}