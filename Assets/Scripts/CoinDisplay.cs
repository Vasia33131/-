using UnityEngine;
using UnityEngine.UI; // Добавьте эту директиву

public class CoinDisplay : MonoBehaviour // Измените имя класса
{
    public Text coinsText; // Теперь это будет ссылаться на UnityEngine.UI.Text

    void Start()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Монеты: {CoinManager.Instance.Coins}";
        }
    }
}