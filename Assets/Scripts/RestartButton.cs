using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    // Этот метод можно назначить на кнопку в Inspector'е
    public void RestartScene()
    {
        // Получаем имя текущей сцены и загружаем её заново
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}