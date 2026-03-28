using UnityEngine;
using TMPro;

public class EndGameTrigger : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI rawTimeText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (!collision.CompareTag("Player")) return;

        triggered = true;
        ShowEndScreen();
    }

    private void ShowEndScreen()
    {
        GameTimer.Instance.StopTimer();

        float raw = GameTimer.Instance.ElapsedTime;
        int coins = AbilityCurrency.Instance.CurrentCoins;
        float bonus = coins * GameTimer.Instance.SecondsPerCoin;
        float final = GameTimer.Instance.GetFinalTime();

        endPanel.SetActive(true);

        if (rawTimeText != null)
            rawTimeText.text = $"Total Time: {GameTimer.FormatTime(raw)}";

        if (coinsText != null)
            coinsText.text = $"Remaining Coins: {coins}";

        if (bonusText != null)
            bonusText.text = $"Bonus: -{GameTimer.FormatTime(bonus)}";

        if (finalTimeText != null)
            finalTimeText.text = $"FINAL TIME: {GameTimer.FormatTime(final)}";
    }
}