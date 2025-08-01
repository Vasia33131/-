using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void LoadGameScene()
    {
        // Загружаем сцену с индексом 1 (GameScene)
        SceneManager.LoadScene(1);

        // ИЛИ по имени (если используете именованные сцены)
        // SceneManager.LoadScene("GameScene");
    }
}