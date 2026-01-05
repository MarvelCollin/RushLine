using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float difficultyIncreaseRate = 0.1f;

    private float score;
    private int diamondCount;
    private int sessionDiamonds;
    private bool isGameOver;
    private float currentDifficulty;
    private GameState currentState;

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
        diamondCount = 0;
        sessionDiamonds = 0;
        isGameOver = false;
        currentDifficulty = 1f;
        currentState = GameState.Menu;
        
        if (FindObjectOfType<MainMenuUI>() == null)
        {
            GameObject menuObj = new GameObject("MainMenu");
            menuObj.AddComponent<MainMenuUI>();
        }
        
        if (UIManager.Instance == null)
        {
            gameObject.AddComponent<UIManager>();
        }
        
        if (PowerUpManager.Instance == null)
        {
            gameObject.AddComponent<PowerUpManager>();
        }
        
        if (AudioManager.Instance == null)
        {
            gameObject.AddComponent<AudioManager>();
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(0);
            UIManager.Instance.UpdateDiamondCount(0);
        }
    }

    void Update()
    {
        if (currentState == GameState.Menu)
        {
            if (FindObjectOfType<MainMenuUI>() == null)
            {
                currentState = GameState.Playing;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBackgroundMusic();
                }
            }
            return;
        }
        
        if (!isGameOver && currentState == GameState.Playing)
        {
            score += Time.deltaTime;
            currentDifficulty = 1f + (score * difficultyIncreaseRate * 0.01f);
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScore(Mathf.FloorToInt(score));
            }
        }
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        isGameOver = true;
        currentState = GameState.GameOver;
        
        PersistentData.AddDiamonds(sessionDiamonds);
        PersistentData.SetHighScore(Mathf.FloorToInt(score));

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(Mathf.FloorToInt(score));
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsPlaying()
    {
        return currentState == GameState.Playing && !isGameOver;
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    public float GetScore()
    {
        return score;
    }

    public float GetDifficulty()
    {
        return currentDifficulty;
    }

    public void AddDiamond()
    {
        int multiplier = 1;
        if (PowerUpManager.Instance != null)
        {
            multiplier = PowerUpManager.Instance.GetDiamondMultiplier();
        }
        
        diamondCount += multiplier;
        sessionDiamonds += multiplier;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateDiamondCount(diamondCount);
        }
    }

    public int GetDiamondCount()
    {
        return diamondCount;
    }

    public int GetSessionDiamonds()
    {
        return sessionDiamonds;
    }

    public int GetTotalDiamonds()
    {
        return PersistentData.GetDiamonds();
    }
}
