using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void LoadGameScene()
    {
        // ��������� ����� � �������� 1 (GameScene)
        SceneManager.LoadScene(1);

        // ��� �� ����� (���� ����������� ����������� �����)
        // SceneManager.LoadScene("GameScene");
    }
}