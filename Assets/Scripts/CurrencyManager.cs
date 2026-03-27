using UnityEngine;
using TMPro;

public class AbilityCurrency : MonoBehaviour
{
    public static AbilityCurrency Instance;

    [SerializeField] private int startingCoins = 5;
    [SerializeField] private TextMeshProUGUI coinText;
    private int currentCoins;

    public int CurrentCoins => currentCoins;

    private void Awake()
    {
        Instance = this;
        currentCoins = startingCoins;
        UpdateUI();
    }

    public bool TrySpend(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
}