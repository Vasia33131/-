using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [Header("Sound Settings")]
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    public AudioClip backgroundMusic;
    [Range(0, 1)] public float soundEffectsVolume = 0.5f;
    [Range(0, 1)] public float musicVolume = 0.3f;

    [Header("UI References")]
    public Image questionImage;
    public Button[] answerButtons;
    public Text coinsText;
    public GameObject gameOverPanel;
    public Text finalCoinsText;
    public Text questionCounterText;
    public Text livesText;
    public GameObject[] lifeIcons;
    public Button hintButton;
    public Color disabledAnswerColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public GameObject gameCompletedPanel;

    private List<AnimeQuestion> unansweredQuestions;
    private AnimeQuestion currentQuestion;
    private AudioSource soundEffectsSource;
    private AudioSource musicSource;
    private int totalQuestions;
    private int questionsAnswered;
    private int currentLives;
    private List<int> disabledAnswerIndices = new List<int>();

    void Start()
    {
        soundEffectsSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();

        soundEffectsSource.volume = soundEffectsVolume;

        if (questions.Count < 4)
        {
            Debug.LogError("Недостаточно вопросов! Нужно минимум 4.");
            return;
        }

        unansweredQuestions = new List<AnimeQuestion>(questions);
        totalQuestions = Mathf.Min(questions.Count, maxQuestions);
        questionsAnswered = 0;
        currentLives = maxLives;

        UpdateQuestionCounter();
        UpdateLivesUI();
        SetRandomQuestion();
        UpdateCoinsUI();
        gameOverPanel.SetActive(false);
        gameCompletedPanel.SetActive(false);

        hintButton.onClick.AddListener(UseHint);
        UpdateHintButton();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void SetRandomQuestion()
    {
        disabledAnswerIndices.Clear();

        if (unansweredQuestions.Count == 0 || currentLives <= 0 || questionsAnswered >= maxQuestions)
        {
            EndGame();
            return;
        }

        int randomIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomIndex];
        unansweredQuestions.RemoveAt(randomIndex);

        questionImage.sprite = currentQuestion.image;

        List<string> allAnswers = new List<string>(currentQuestion.wrongAnswers);
        allAnswers.Add(currentQuestion.correctAnswer);

        for (int i = 0; i < allAnswers.Count; i++)
        {
            string temp = allAnswers[i];
            int random = Random.Range(i, allAnswers.Count);
            allAnswers[i] = allAnswers[random];
            allAnswers[random] = temp;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Text>().text = allAnswers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].interactable = true;

            var colors = answerButtons[i].colors;
            colors.normalColor = Color.white;
            answerButtons[i].colors = colors;

            string answer = allAnswers[i];
            answerButtons[i].onClick.AddListener(() => CheckAnswer(answer));
        }

        UpdateHintButton();
    }

    void CheckAnswer(string selectedAnswer)
    {
        if (selectedAnswer == currentQuestion.correctAnswer)
        {
            CoinManager.Instance.AddCoins(coinsPerCorrectAnswer);
            PlaySound(correctAnswerSound);
            Debug.Log($"Правильно! +{coinsPerCorrectAnswer} монет");
        }
        else
        {
            currentLives--;
            UpdateLivesUI();
            PlaySound(wrongAnswerSound);
            Debug.Log($"Неправильно! Правильный ответ: {currentQuestion.correctAnswer}");

            if (currentLives <= 0)
            {
                EndGame();
                return;
            }
        }

        questionsAnswered++;
        UpdateQuestionCounter();
        UpdateCoinsUI();

        if (questionsAnswered >= maxQuestions)
        {
            EndGame();
        }
        else
        {
            Invoke("SetRandomQuestion", 1f);
        }
    }

    void UseHint()
    {
        if (CoinManager.Instance.SpendCoins(hintCost))
        {
            List<int> wrongAnswerIndices = new List<int>();
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i].GetComponentInChildren<Text>().text != currentQuestion.correctAnswer &&
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
        else
        {
            Debug.Log("Недостаточно монет для подсказки!");
        }
    }

    void UpdateHintButton()
    {
        hintButton.interactable = CoinManager.Instance.Coins >= hintCost &&
                               disabledAnswerIndices.Count < answerButtons.Length - 2;
    }

    void UpdateLivesUI()
    {
        livesText.text = $"Жизни: {currentLives}/{maxLives}";

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
        coinsText.text = $"Монеты: {CoinManager.Instance.Coins}";
        UpdateHintButton();
    }

    void UpdateQuestionCounter()
    {
        questionCounterText.text = $"Вопрос: {questionsAnswered + 1}/{totalQuestions}";
    }

    void EndGame()
    {
        if (questionsAnswered >= maxQuestions)
        {
            gameCompletedPanel.SetActive(true);
            finalCoinsText.text = $"Финальные монеты: {CoinManager.Instance.Coins}";
        }
        else
        {
            gameOverPanel.SetActive(true);
            finalCoinsText.text = $"Финальные монеты: {CoinManager.Instance.Coins}";
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
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