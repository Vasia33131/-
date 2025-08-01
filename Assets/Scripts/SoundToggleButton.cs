using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnSprite;    // Спрайт, когда звук включен
    public Sprite soundOffSprite;   // Спрайт, когда звук выключен
    private Image buttonImage;     // Компонент Image кнопки
    private bool isSoundOn = true; // Флаг состояния звука

    void Start()
    {
        // Получаем компонент Image кнопки
        buttonImage = GetComponent<Image>();

        // Загружаем сохранённое состояние звука (1 - вкл, 0 - выкл)
        isSoundOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        // Применяем настройки звука
        ApplySoundSettings();

        // Обновляем спрайт кнопки
        UpdateButtonSprite();
    }

    // Метод, вызываемый при нажатии на кнопку
    public void OnButtonClick()
    {
        // Переключаем состояние звука
        isSoundOn = !isSoundOn;

        // Сохраняем новое состояние
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        // Применяем настройки звука
        ApplySoundSettings();

        // Обновляем спрайт кнопки
        UpdateButtonSprite();
    }

    // Применяем текущие настройки звука
    private void ApplySoundSettings()
    {
        AudioListener.pause = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1f : 0f;
    }

    // Обновляем спрайт кнопки в зависимости от состояния звука
    private void UpdateButtonSprite()
    {
        buttonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
    }
}