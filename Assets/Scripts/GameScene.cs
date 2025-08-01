using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void LoadMenuScene()
    {
        // Загружаем сцену с индексом 0 (MainMenu)
        SceneManager.LoadScene(0);

        // ИЛИ по имени
        // SceneManager.LoadScene("MainMenu");
    }
}