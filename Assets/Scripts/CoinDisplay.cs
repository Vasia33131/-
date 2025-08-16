using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    void Start()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Монеты: {CoinManager.Instance.Coins}";
        }
    }
}