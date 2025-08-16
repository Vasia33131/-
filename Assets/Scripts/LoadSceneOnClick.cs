using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour
{
    public int sceneIndex;
    public Button loadSceneButton; // Ссылка на кнопку, которая будет затемняться

    private void Start()
    {
        // Проверяем наличие кнопки
        if (loadSceneButton != null)
        {
            // Проверяем достаточно ли монет при старте
            UpdateButtonState();

            // Добавляем слушатель нажатия
            loadSceneButton.onClick.AddListener(LoadTargetScene);
        }
        else
        {
            Debug.LogError("Load Scene Button reference is missing!");
        }
    }

    public void LoadTargetScene()
    {
        // Проверяем достаточно ли монет
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(250))
        {
            // Проверяем, чтобы индекс был в допустимых пределах
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

        // Проверяем достаточно ли монет и обновляем состояние кнопки
        bool canAfford = CoinManager.Instance != null && CoinManager.Instance.Coins >= 250;
        loadSceneButton.interactable = canAfford;

        // Можно также изменить цвет, если нужно
        var colors = loadSceneButton.colors;
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Серый полупрозрачный
        loadSceneButton.colors = colors;
    }
}