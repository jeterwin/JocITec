using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int secondsPerCoin = 2;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    public float ElapsedTime => elapsedTime;
    public int SecondsPerCoin => secondsPerCoin;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateUI();
    }

    // Returneaza timpul final dupa ce scazi bonusul din coins
    public float GetFinalTime()
    {
        int coins = AbilityCurrency.Instance.CurrentCoins;
        float bonus = coins * secondsPerCoin;
        float finalTime = Mathf.Max(0f, elapsedTime - bonus);
        return finalTime;
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = FormatTime(elapsedTime);
    }

    public static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        int ms = Mathf.FloorToInt((seconds * 100f) % 100f);
        return $"{m:00}:{s:00}.{ms:00}";
    }
}