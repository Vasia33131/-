using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private int maxLives = 3; // ������������ ���������� ������
    private int currentLives; // ������� ���������� ������

    private void Start()
    {
        currentLives = maxLives; // �������������� ����� ��� ������
    }

    // ����� ��� ���������� ������ (���������� ��� ��������� �����/���������)
    public void LoseLife()
    {
        currentLives--;

        // ���������, �������� �� �����
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    // ����� ��� ��������� ���������
    private void GameOver()
    {
        // ��������� ����� �������� ���� (���������, ��� ��� ��������� � Build Settings)
        SceneManager.LoadScene(0);

        // ������������� ����� ������������ ������ �����:
        // SceneManager.LoadScene(0); // ��� 0 - ������ �������� ����
    }

    // �����������: ����� ��� �������� ������� ������ (��������, ��� UI)
    public int GetCurrentLives()
    {
        return currentLives;
    }
}