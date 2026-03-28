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

    [Header("Transition (Match GameManager)")]
    [SerializeField] private GameObject deathScreen; // Drag your Death Screen here
    [SerializeField] private Animator deathAnimator; // Drag your Death Animator here
    [SerializeField] private float transitionTime = 0.5f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered || !collision.CompareTag("Player")) return;
        triggered = true;
        ShowEndScreen();
        GoToMainMenu();
    }

    private void ShowEndScreen()
    {
        GameTimer.Instance.StopTimer();

        float raw = GameTimer.Instance.ElapsedTime;
        int coins = AbilityCurrency.Instance.CurrentCoins;
        float bonus = coins * GameTimer.Instance.SecondsPerCoin;
        float final = GameTimer.Instance.GetFinalTime();

        endPanel.SetActive(true);

        if (rawTimeText != null) rawTimeText.text = $"Total Time: {GameTimer.FormatTime(raw)}";
        if (coinsText != null) coinsText.text = $"Remaining Coins: {coins}";
        if (bonusText != null) bonusText.text = $"Bonus: -{GameTimer.FormatTime(bonus)}";
        if (finalTimeText != null) finalTimeText.text = $"FINAL TIME: {GameTimer.FormatTime(final)}";
    }

    // Call this from your "Main Menu" button onClick()
    public void GoToMainMenu()
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        yield return new WaitForSeconds(transitionTime);

        deathScreen.SetActive(true);

        deathAnimator.Play("FadeIn");


        endPanel.SetActive(false);

        // 5. Swap scenes
        SceneManager.LoadScene("Main Menu");
    }
}