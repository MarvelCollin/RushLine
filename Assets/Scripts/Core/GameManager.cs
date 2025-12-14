using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float difficultyIncreaseRate = 0.1f;

    private float score;
    private int diamondCount;
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
        diamondCount = 0;
        isGameOver = false;
        currentDifficulty = 1f;
        
        if (UIManager.Instance == null)
        {
            gameObject.AddComponent<UIManager>();
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(0);
            UIManager.Instance.UpdateDiamondCount(0);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            score += Time.deltaTime;
            currentDifficulty = 1f + (score * difficultyIncreaseRate * 0.01f);
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScore(Mathf.FloorToInt(score));
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(Mathf.FloorToInt(score));
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

    public void AddDiamond()
    {
        diamondCount++;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateDiamondCount(diamondCount);
        }
    }

    public int GetDiamondCount()
    {
        return diamondCount;
    }
}
