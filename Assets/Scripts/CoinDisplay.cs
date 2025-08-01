using UnityEngine;
using UnityEngine.UI; // �������� ��� ���������

public class CoinDisplay : MonoBehaviour // �������� ��� ������
{
    public Text coinsText; // ������ ��� ����� ��������� �� UnityEngine.UI.Text

    void Start()
    {
        if (coinsText != null)
        {
            coinsText.text = $"������: {CoinManager.Instance.Coins}";
        }
    }
}