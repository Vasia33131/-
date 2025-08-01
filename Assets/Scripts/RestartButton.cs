using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    // ���� ����� ����� ��������� �� ������ � Inspector'�
    public void RestartScene()
    {
        // �������� ��� ������� ����� � ��������� � ������
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}