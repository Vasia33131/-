using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private int maxLives = 3; // Максимальное количество жизней
    private int currentLives; // Текущее количество жизней

    private void Start()
    {
        currentLives = maxLives; // Инициализируем жизни при старте
    }

    // Метод для уменьшения жизней (вызывается при получении урона/проигрыше)
    public void LoseLife()
    {
        currentLives--;

        // Проверяем, остались ли жизни
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    // Метод для обработки проигрыша
    private void GameOver()
    {
        // Загружаем сцену главного меню (убедитесь, что она добавлена в Build Settings)
        SceneManager.LoadScene(0);

        // Альтернативно можно использовать индекс сцены:
        // SceneManager.LoadScene(0); // где 0 - индекс главного меню
    }

    // Опционально: метод для проверки текущих жизней (например, для UI)
    public int GetCurrentLives()
    {
        return currentLives;
    }
}