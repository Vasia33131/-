using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour
{
    public int sceneIndex;
    public Button loadSceneButton; // ������ �� ������, ������� ����� �����������

    private void Start()
    {
        // ��������� ������� ������
        if (loadSceneButton != null)
        {
            // ��������� ���������� �� ����� ��� ������
            UpdateButtonState();

            // ��������� ��������� �������
            loadSceneButton.onClick.AddListener(LoadTargetScene);
        }
        else
        {
            Debug.LogError("Load Scene Button reference is missing!");
        }
    }

    public void LoadTargetScene()
    {
        // ��������� ���������� �� �����
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(250))
        {
            // ���������, ����� ������ ��� � ���������� ��������
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogError("Invalid scene index!");
            }
        }
        else
        {
            Debug.Log("Not enough coins to load scene! Need 250 coins.");
        }
    }

    private void UpdateButtonState()
    {
        if (loadSceneButton == null) return;

        // ��������� ���������� �� ����� � ��������� ��������� ������
        bool canAfford = CoinManager.Instance != null && CoinManager.Instance.Coins >= 250;
        loadSceneButton.interactable = canAfford;

        // ����� ����� �������� ����, ���� �����
        var colors = loadSceneButton.colors;
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // ����� ��������������
        loadSceneButton.colors = colors;
    }
}