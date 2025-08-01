using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnSprite;    // ������, ����� ���� �������
    public Sprite soundOffSprite;   // ������, ����� ���� ��������
    private Image buttonImage;     // ��������� Image ������
    private bool isSoundOn = true; // ���� ��������� �����

    void Start()
    {
        // �������� ��������� Image ������
        buttonImage = GetComponent<Image>();

        // ��������� ���������� ��������� ����� (1 - ���, 0 - ����)
        isSoundOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        // ��������� ��������� �����
        ApplySoundSettings();

        // ��������� ������ ������
        UpdateButtonSprite();
    }

    // �����, ���������� ��� ������� �� ������
    public void OnButtonClick()
    {
        // ����������� ��������� �����
        isSoundOn = !isSoundOn;

        // ��������� ����� ���������
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        // ��������� ��������� �����
        ApplySoundSettings();

        // ��������� ������ ������
        UpdateButtonSprite();
    }

    // ��������� ������� ��������� �����
    private void ApplySoundSettings()
    {
        AudioListener.pause = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1f : 0f;
    }

    // ��������� ������ ������ � ����������� �� ��������� �����
    private void UpdateButtonSprite()
    {
        buttonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
    }
}