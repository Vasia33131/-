using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void LoadMenuScene()
    {
        // ��������� ����� � �������� 0 (MainMenu)
        SceneManager.LoadScene(0);

        // ��� �� �����
        // SceneManager.LoadScene("MainMenu");
    }
}