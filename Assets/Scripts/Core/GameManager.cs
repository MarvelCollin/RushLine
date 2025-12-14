using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public Text scoreText;
    public GameObject gameOverPanel;
    public Text gameOverScoreText;
    public Button restartButton;

    [Header("Game Settings")]
    public float difficultyIncreaseRate = 0.1f;

    private float score;
    private bool isGameOver;
    private float currentDifficulty;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        isGameOver = false;
        currentDifficulty = 1f;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            score += Time.deltaTime;
            currentDifficulty = 1f + (score * difficultyIncreaseRate * 0.01f);
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public float GetScore()
    {
        return score;
    }

    public float GetDifficulty()
    {
        return currentDifficulty;
    }
}
