using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimeQuizGame : MonoBehaviour
{
    [System.Serializable]
    public class AnimeQuestion
    {
        public Sprite image;
        public string correctAnswer;
        public string[] wrongAnswers;
    }

    [Header("Game Settings")]
    public List<AnimeQuestion> questions;
    public int coinsPerCorrectAnswer = 10;
    public int maxLives = 3;
    public int hintCost = 100;
    public int maxQuestions = 50;
    public string nextSceneName = "NextScene";
    public bool isFinalScene = false; // ���� ��� ����������� ��������� �����

    [Header("Sound Settings")]
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    public AudioClip backgroundMusic;
    [Range(0, 1)] public float soundEffectsVolume = 0.5f;
    [Range(0, 1)] public float musicVolume = 0.3f;

    [Header("UI References")]
    public Image questionImage;
    public Button[] answerButtons;
    public TextMeshProUGUI coinsText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI[] finalCoinTexts; // ������ ��������� ��������� ��� ����������� ��������� �����
    public TextMeshProUGUI questionCounterText;
    public TextMeshProUGUI livesText;
    public GameObject[] lifeIcons;
    public Button hintButton;
    public Color disabledAnswerColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public GameObject victoryPanel;
    public Button continueButton;
    public Button restartButton;
    public Button menuButton;

    private List<AnimeQuestion> unansweredQuestions;
    private AnimeQuestion currentQuestion;
    private AudioSource soundEffectsSource;
    private AudioSource musicSource;
    private int totalQuestions;
    private int questionsAnswered;
    private int currentLives;
    private List<int> disabledAnswerIndices = new List<int>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeGame();
        SetupUIForScene();
    }

    void SetupUIForScene()
    {
        if (isFinalScene && continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }

        if (restartButton != null) restartButton.gameObject.SetActive(true);
        if (menuButton != null) menuButton.gameObject.SetActive(true);
    }

    void InitializeGame()
    {
        // �������� ���� ����������� ������
        if (questionImage == null) Debug.LogError("QuestionImage reference is missing!");
        if (answerButtons == null || answerButtons.Length == 0) Debug.LogError("Answer Buttons are not assigned!");
        if (coinsText == null) Debug.LogError("Coins Text reference is missing!");
        if (gameOverPanel == null) Debug.LogError("Game Over Panel reference is missing!");
        if (victoryPanel == null) Debug.LogError("Victory Panel reference is missing!");

        // ������������� �����
        soundEffectsSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();

        soundEffectsSource.volume = soundEffectsVolume;

        // �������� ��������
        if (questions.Count < 4)
        {
            Debug.LogError("Not enough questions! Minimum 4 required.");
            return;
        }

        // ������������� ����
        unansweredQuestions = new List<AnimeQuestion>(questions);
        totalQuestions = Mathf.Min(questions.Count, maxQuestions);
        questionsAnswered = 0;
        currentLives = maxLives;

        // ��������� UI
        UpdateQuestionCounter();
        UpdateLivesUI();
        UpdateCoinsUI();

        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        // ���������� ������������ ������
        hintButton.onClick.AddListener(UseHint);
        if (continueButton != null) continueButton.onClick.AddListener(ContinueToNextScene);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null) menuButton.onClick.AddListener(ReturnToMenu);

        UpdateHintButton();
        UpdateContinueButton();

        // ������ ���� final
        SetRandomQuestion();
    }

    void SetRandomQuestion()
    {
        if (questionImage == null) return;

        disabledAnswerIndices.Clear();

        // �������� ������� ��������� ����
        if (unansweredQuestions.Count == 0 || questionsAnswered >= maxQuestions)
        {
            EndGame(true); // ������ - ��� ������� ��������
            return;
        }

        if (currentLives <= 0)
        {
            EndGame(false); // ��������� - ����������� �����
            return;
        }

        // ����� ���������� �������
        int randomIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomIndex];
        unansweredQuestions.RemoveAt(randomIndex);

        // ��������� ����������� �������
        questionImage.sprite = currentQuestion?.image;

        // ���������� ��������� �������
        List<string> allAnswers = new List<string>(currentQuestion.wrongAnswers);
        allAnswers.Add(currentQuestion.correctAnswer);

        // ������������� �������
        for (int i = 0; i < allAnswers.Count; i++)
        {
            int random = Random.Range(i, allAnswers.Count);
            string temp = allAnswers[i];
            allAnswers[i] = allAnswers[random];
            allAnswers[random] = temp;
        }

        // ��������� ������ �������
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            var buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = i < allAnswers.Count ? allAnswers[i] : "N/A";

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].interactable = true;

            var colors = answerButtons[i].colors;
            colors.normalColor = Color.white;
            answerButtons[i].colors = colors;

            if (i < allAnswers.Count)
            {
                string answer = allAnswers[i];
                answerButtons[i].onClick.AddListener(() => CheckAnswer(answer));
            }
        }

        UpdateHintButton();
    }

    void CheckAnswer(string selectedAnswer)
    {
        if (selectedAnswer == currentQuestion.correctAnswer)
        {
            // ���������� �����
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoins(coinsPerCorrectAnswer);
            }
            PlaySound(correctAnswerSound);
        }
        else
        {
            // ������������ �����
            currentLives--;
            UpdateLivesUI();
            PlaySound(wrongAnswerSound);

            if (currentLives <= 0)
            {
                EndGame(false); // ���������
                return;
            }
        }

        questionsAnswered++;
        UpdateQuestionCounter();
        UpdateCoinsUI();

        if (questionsAnswered >= maxQuestions || unansweredQuestions.Count == 0)
        {
            EndGame(true); // ������ - ��� ������� ��������
        }
        else if (currentLives <= 0)
        {
            EndGame(false); // ���������
        }
        else
        {
            Invoke("SetRandomQuestion", 1f);
        }
    }

    void EndGame(bool isVictory)
    {
        if (isVictory)
        {
            victoryPanel.SetActive(true);
            int finalCoins = CoinManager.Instance?.Coins ?? 0;

            // ��������� ��� ��������� �������� � ������� finalCoinTexts
            foreach (var text in finalCoinTexts)
            {
                if (text != null)
                {
                    text.text = $"������������ ������: {finalCoins}";
                }
            }
        }
        else
        {
            gameOverPanel.SetActive(true);
            int finalCoins = CoinManager.Instance?.Coins ?? 0;

            if (finalCoinTexts.Length > 0 && finalCoinTexts[0] != null)
            {
                finalCoinTexts[0].text = $"������������ ������: {finalCoins}";
            }
        }

        UpdateContinueButton();
    }

    // ��������� ������ �������� ��� ���������
    void UseHint()
    {
        if (CoinManager.Instance == null || !CoinManager.Instance.SpendCoins(hintCost))
        {
            Debug.Log("Not enough coins for hint!");
            return;
        }

        List<int> wrongAnswerIndices = new List<int>();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            var buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null && buttonText.text != currentQuestion.correctAnswer &&
                !disabledAnswerIndices.Contains(i))
            {
                wrongAnswerIndices.Add(i);
            }
        }

        int hintsToShow = Mathf.Min(2, wrongAnswerIndices.Count);
        for (int i = 0; i < hintsToShow; i++)
        {
            int randomIndex = Random.Range(0, wrongAnswerIndices.Count);
            int buttonIndex = wrongAnswerIndices[randomIndex];

            answerButtons[buttonIndex].interactable = false;
            var colors = answerButtons[buttonIndex].colors;
            colors.disabledColor = disabledAnswerColor;
            answerButtons[buttonIndex].colors = colors;

            disabledAnswerIndices.Add(buttonIndex);
            wrongAnswerIndices.RemoveAt(randomIndex);
        }

        UpdateCoinsUI();
        UpdateHintButton();
    }

    void UpdateHintButton()
    {
        if (hintButton == null) return;

        bool canUseHint = CoinManager.Instance != null &&
                         CoinManager.Instance.Coins >= hintCost &&
                         disabledAnswerIndices.Count < answerButtons.Length - 2;

        hintButton.interactable = canUseHint;
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"�����: {currentLives}/{maxLives}";
        }

        if (lifeIcons != null && lifeIcons.Length >= maxLives)
        {
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null)
                {
                    lifeIcons[i].SetActive(i < currentLives);
                }
            }
        }
    }

    void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = $"������: {CoinManager.Instance?.Coins ?? 0}";
        }
        UpdateHintButton();
    }

    void UpdateQuestionCounter()
    {
        if (questionCounterText != null)
        {
            questionCounterText.text = $"������: {questionsAnswered + 1}/{totalQuestions}";
        }
    }

    void UpdateContinueButton()
    {
        if (continueButton == null) return;

        continueButton.interactable = currentLives > 0;

        var colors = continueButton.colors;
        colors.disabledColor = Color.gray;
        continueButton.colors = colors;
    }

    void ContinueToNextScene()
    {
        if (currentLives > 0 && !string.IsNullOrEmpty(nextSceneName) && !isFinalScene)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && soundEffectsSource != null)
        {
            soundEffectsSource.PlayOneShot(clip);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}