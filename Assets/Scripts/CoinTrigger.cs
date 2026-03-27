using UnityEngine;

public class CoinTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AbilityCurrency.Instance.AddCoins(1);
        Destroy(gameObject);
    }
}
