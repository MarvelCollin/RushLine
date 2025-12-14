using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Canvas canvas;
    private GameObject scorePanel;
    private Text scoreText;
    private GameObject diamondPanel;
    private Text diamondText;
    private GameObject gameOverPanel;
    private Text gameOverTitle;
    private Text finalScoreText;
    private Text finalDiamondText;
    private Text highScoreText;
    private Button retryButton;
    private Button quitButton;

    private int highScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (GameManager.Instance == null)
        {
            gameObject.AddComponent<GameManager>();
        }
        
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        CreateUI();
        CreateEventSystem();
    }

    void Start()
    {
    }

    void CreateEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    void CreateUI()
    {
        GameObject canvasObj = new GameObject("GameCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1f;
        canvas.sortingOrder = 999;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();

        CreateScorePanel();
        CreateDiamondPanel();
        CreateGameOverPanel();
        
        scorePanel.SetActive(true);
        diamondPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    void CreateScorePanel()
    {
        scorePanel = new GameObject("ScorePanel");
        scorePanel.transform.SetParent(canvas.transform, false);
        
        RectTransform scorePanelRect = scorePanel.AddComponent<RectTransform>();
        scorePanelRect.anchorMin = new Vector2(0f, 1f);
        scorePanelRect.anchorMax = new Vector2(0f, 1f);
        scorePanelRect.pivot = new Vector2(0f, 1f);
        scorePanelRect.anchoredPosition = new Vector2(25, -25);
        scorePanelRect.sizeDelta = new Vector2(220, 70);

        Image scoreBg = scorePanel.AddComponent<Image>();
        scoreBg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        GameObject borderLeft = new GameObject("BorderLeft");
        borderLeft.transform.SetParent(scorePanel.transform, false);
        RectTransform borderLeftRect = borderLeft.AddComponent<RectTransform>();
        borderLeftRect.anchorMin = new Vector2(0, 0);
        borderLeftRect.anchorMax = new Vector2(0, 1);
        borderLeftRect.pivot = new Vector2(0, 0.5f);
        borderLeftRect.anchoredPosition = Vector2.zero;
        borderLeftRect.sizeDelta = new Vector2(4, 0);
        Image borderLeftImg = borderLeft.AddComponent<Image>();
        borderLeftImg.color = new Color(1f, 0.8f, 0.2f, 1f);

        GameObject scoreLabel = new GameObject("ScoreLabel");
        scoreLabel.transform.SetParent(scorePanel.transform, false);
        RectTransform scoreLabelRect = scoreLabel.AddComponent<RectTransform>();
        scoreLabelRect.anchorMin = new Vector2(0, 0);
        scoreLabelRect.anchorMax = new Vector2(0, 1);
        scoreLabelRect.pivot = new Vector2(0, 0.5f);
        scoreLabelRect.anchoredPosition = new Vector2(12, 0);
        scoreLabelRect.sizeDelta = new Vector2(70, 0);
        
        Text scoreLabelText = scoreLabel.AddComponent<Text>();
        scoreLabelText.text = "SCORE";
        scoreLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreLabelText.fontSize = 18;
        scoreLabelText.fontStyle = FontStyle.Bold;
        scoreLabelText.alignment = TextAnchor.MiddleLeft;
        scoreLabelText.color = new Color(1f, 0.8f, 0.2f, 1f);

        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(scorePanel.transform, false);
        
        RectTransform scoreTextRect = scoreTextObj.AddComponent<RectTransform>();
        scoreTextRect.anchorMin = new Vector2(0, 0);
        scoreTextRect.anchorMax = new Vector2(1, 1);
        scoreTextRect.offsetMin = new Vector2(70, 0);
        scoreTextRect.offsetMax = new Vector2(-15, 0);
        
        scoreText = scoreTextObj.AddComponent<Text>();
        scoreText.text = "0";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 42;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.alignment = TextAnchor.MiddleRight;
        scoreText.color = Color.white;

        Outline scoreOutline = scoreTextObj.AddComponent<Outline>();
        scoreOutline.effectColor = new Color(0, 0, 0, 1f);
        scoreOutline.effectDistance = new Vector2(2, -2);
    }

    void CreateDiamondPanel()
    {
        diamondPanel = new GameObject("DiamondPanel");
        diamondPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform diamondPanelRect = diamondPanel.AddComponent<RectTransform>();
        diamondPanelRect.anchorMin = new Vector2(1f, 1f);
        diamondPanelRect.anchorMax = new Vector2(1f, 1f);
        diamondPanelRect.pivot = new Vector2(1f, 1f);
        diamondPanelRect.anchoredPosition = new Vector2(-25, -25);
        diamondPanelRect.sizeDelta = new Vector2(180, 70);

        Image diamondBg = diamondPanel.AddComponent<Image>();
        diamondBg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        GameObject borderRight = new GameObject("BorderRight");
        borderRight.transform.SetParent(diamondPanel.transform, false);
        RectTransform borderRightRect = borderRight.AddComponent<RectTransform>();
        borderRightRect.anchorMin = new Vector2(1, 0);
        borderRightRect.anchorMax = new Vector2(1, 1);
        borderRightRect.pivot = new Vector2(1, 0.5f);
        borderRightRect.anchoredPosition = Vector2.zero;
        borderRightRect.sizeDelta = new Vector2(4, 0);
        Image borderRightImg = borderRight.AddComponent<Image>();
        borderRightImg.color = new Color(0.3f, 0.85f, 1f, 1f);

        GameObject diamondIcon = new GameObject("DiamondIcon");
        diamondIcon.transform.SetParent(diamondPanel.transform, false);
        RectTransform diamondIconRect = diamondIcon.AddComponent<RectTransform>();
        diamondIconRect.anchorMin = new Vector2(0, 0.5f);
        diamondIconRect.anchorMax = new Vector2(0, 0.5f);
        diamondIconRect.pivot = new Vector2(0, 0.5f);
        diamondIconRect.anchoredPosition = new Vector2(12, 0);
        diamondIconRect.sizeDelta = new Vector2(46, 46);
        
        Image diamondIconImg = diamondIcon.AddComponent<Image>();
        Sprite diamondSprite = Resources.Load<Sprite>("Diamond");
        if (diamondSprite != null)
        {
            diamondIconImg.sprite = diamondSprite;
            diamondIconImg.preserveAspect = true;
        }
        else
        {
            diamondIconImg.color = new Color(0.3f, 0.85f, 1f, 1f);
        }

        GameObject diamondTextObj = new GameObject("DiamondText");
        diamondTextObj.transform.SetParent(diamondPanel.transform, false);
        
        RectTransform diamondTextRect = diamondTextObj.AddComponent<RectTransform>();
        diamondTextRect.anchorMin = new Vector2(0, 0);
        diamondTextRect.anchorMax = new Vector2(1, 1);
        diamondTextRect.offsetMin = new Vector2(65, 0);
        diamondTextRect.offsetMax = new Vector2(-15, 0);
        
        diamondText = diamondTextObj.AddComponent<Text>();
        diamondText.text = "0";
        diamondText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        diamondText.fontSize = 38;
        diamondText.fontStyle = FontStyle.Bold;
        diamondText.alignment = TextAnchor.MiddleRight;
        diamondText.color = Color.white;

        Outline diamondOutline = diamondTextObj.AddComponent<Outline>();
        diamondOutline.effectColor = new Color(0, 0, 0, 1f);
        diamondOutline.effectDistance = new Vector2(2, -2);
    }

    void CreateGameOverPanel()
    {
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image overlay = gameOverPanel.AddComponent<Image>();
        overlay.color = new Color(0, 0, 0, 0.85f);

        GameObject modalBox = new GameObject("ModalBox");
        modalBox.transform.SetParent(gameOverPanel.transform, false);
        
        RectTransform modalRect = modalBox.AddComponent<RectTransform>();
        modalRect.anchorMin = new Vector2(0.5f, 0.5f);
        modalRect.anchorMax = new Vector2(0.5f, 0.5f);
        modalRect.pivot = new Vector2(0.5f, 0.5f);
        modalRect.sizeDelta = new Vector2(450, 420);
        
        Image modalBg = modalBox.AddComponent<Image>();
        modalBg.color = new Color(0.12f, 0.12f, 0.18f, 1f);

        GameObject borderTop = new GameObject("BorderTop");
        borderTop.transform.SetParent(modalBox.transform, false);
        RectTransform borderTopRect = borderTop.AddComponent<RectTransform>();
        borderTopRect.anchorMin = new Vector2(0, 1);
        borderTopRect.anchorMax = new Vector2(1, 1);
        borderTopRect.pivot = new Vector2(0.5f, 1);
        borderTopRect.anchoredPosition = Vector2.zero;
        borderTopRect.sizeDelta = new Vector2(0, 5);
        Image borderTopImg = borderTop.AddComponent<Image>();
        borderTopImg.color = new Color(0.9f, 0.3f, 0.3f, 1f);

        GameObject borderBottom = new GameObject("BorderBottom");
        borderBottom.transform.SetParent(modalBox.transform, false);
        RectTransform borderBottomRect = borderBottom.AddComponent<RectTransform>();
        borderBottomRect.anchorMin = new Vector2(0, 0);
        borderBottomRect.anchorMax = new Vector2(1, 0);
        borderBottomRect.pivot = new Vector2(0.5f, 0);
        borderBottomRect.anchoredPosition = Vector2.zero;
        borderBottomRect.sizeDelta = new Vector2(0, 5);
        Image borderBottomImg = borderBottom.AddComponent<Image>();
        borderBottomImg.color = new Color(0.9f, 0.3f, 0.3f, 1f);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -35);
        titleRect.sizeDelta = new Vector2(400, 60);
        
        gameOverTitle = titleObj.AddComponent<Text>();
        gameOverTitle.text = "GAME OVER";
        gameOverTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        gameOverTitle.fontSize = 52;
        gameOverTitle.fontStyle = FontStyle.Bold;
        gameOverTitle.alignment = TextAnchor.MiddleCenter;
        gameOverTitle.color = new Color(0.95f, 0.35f, 0.35f, 1f);

        Outline titleOutline = titleObj.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0, 0, 0, 1f);
        titleOutline.effectDistance = new Vector2(2, -2);

        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(modalBox.transform, false);
        RectTransform dividerRect = divider.AddComponent<RectTransform>();
        dividerRect.anchorMin = new Vector2(0.5f, 1f);
        dividerRect.anchorMax = new Vector2(0.5f, 1f);
        dividerRect.pivot = new Vector2(0.5f, 0.5f);
        dividerRect.anchoredPosition = new Vector2(0, -105);
        dividerRect.sizeDelta = new Vector2(350, 2);
        Image dividerImg = divider.AddComponent<Image>();
        dividerImg.color = new Color(0.3f, 0.3f, 0.4f, 1f);

        GameObject scoreObj = new GameObject("FinalScore");
        scoreObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform scoreRect = scoreObj.AddComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.5f, 1f);
        scoreRect.anchorMax = new Vector2(0.5f, 1f);
        scoreRect.pivot = new Vector2(0.5f, 1f);
        scoreRect.anchoredPosition = new Vector2(0, -120);
        scoreRect.sizeDelta = new Vector2(350, 45);
        
        finalScoreText = scoreObj.AddComponent<Text>();
        finalScoreText.text = "SCORE: 0";
        finalScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalScoreText.fontSize = 36;
        finalScoreText.fontStyle = FontStyle.Bold;
        finalScoreText.alignment = TextAnchor.MiddleCenter;
        finalScoreText.color = Color.white;

        GameObject highScoreObj = new GameObject("HighScore");
        highScoreObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform highScoreRect = highScoreObj.AddComponent<RectTransform>();
        highScoreRect.anchorMin = new Vector2(0.5f, 1f);
        highScoreRect.anchorMax = new Vector2(0.5f, 1f);
        highScoreRect.pivot = new Vector2(0.5f, 1f);
        highScoreRect.anchoredPosition = new Vector2(0, -170);
        highScoreRect.sizeDelta = new Vector2(350, 35);
        
        highScoreText = highScoreObj.AddComponent<Text>();
        highScoreText.text = "BEST: 0";
        highScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        highScoreText.fontSize = 28;
        highScoreText.alignment = TextAnchor.MiddleCenter;
        highScoreText.color = new Color(1f, 0.8f, 0.2f, 1f);

        GameObject finalDiamondObj = new GameObject("FinalDiamond");
        finalDiamondObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform finalDiamondRect = finalDiamondObj.AddComponent<RectTransform>();
        finalDiamondRect.anchorMin = new Vector2(0.5f, 1f);
        finalDiamondRect.anchorMax = new Vector2(0.5f, 1f);
        finalDiamondRect.pivot = new Vector2(0.5f, 1f);
        finalDiamondRect.anchoredPosition = new Vector2(0, -210);
        finalDiamondRect.sizeDelta = new Vector2(350, 35);
        
        finalDiamondText = finalDiamondObj.AddComponent<Text>();
        finalDiamondText.text = "◆ 0";
        finalDiamondText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalDiamondText.fontSize = 28;
        finalDiamondText.alignment = TextAnchor.MiddleCenter;
        finalDiamondText.color = new Color(0.3f, 0.85f, 1f, 1f);

        retryButton = CreateButton(modalBox.transform, "RetryButton", "RETRY", new Vector2(0, -275), new Color(0.25f, 0.75f, 0.35f, 1f));
        retryButton.onClick.AddListener(OnRetryClicked);

        quitButton = CreateButton(modalBox.transform, "QuitButton", "QUIT", new Vector2(0, -345), new Color(0.75f, 0.25f, 0.25f, 1f));
        quitButton.onClick.AddListener(OnQuitClicked);

        gameOverPanel.SetActive(false);
    }

    Button CreateButton(Transform parent, string name, string text, Vector2 position, Color bgColor)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 1f);
        buttonRect.anchorMax = new Vector2(0.5f, 1f);
        buttonRect.pivot = new Vector2(0.5f, 1f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(260, 55);
        
        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = bgColor;
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.selectedColor = Color.white;
        button.colors = colors;

        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = buttonTextObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 32;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;

        Outline textOutline = buttonTextObj.AddComponent<Outline>();
        textOutline.effectColor = new Color(0, 0, 0, 0.6f);
        textOutline.effectDistance = new Vector2(1, -1);

        return button;
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateDiamondCount(int count)
    {
        if (diamondText != null)
        {
            diamondText.text = count.ToString();
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "SCORE: " + finalScore.ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = "BEST: " + highScore.ToString();
        }

        if (finalDiamondText != null && GameManager.Instance != null)
        {
            finalDiamondText.text = "◆ " + GameManager.Instance.GetDiamondCount().ToString();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void OnRetryClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnQuitClicked()
    {
        Application.Quit();
    }

    public Vector3 GetDiamondPanelWorldPosition()
    {
        if (diamondPanel != null)
        {
            RectTransform rt = diamondPanel.GetComponent<RectTransform>();
            return rt.position;
        }
        return new Vector3(Screen.width - 100, Screen.height - 50, 0);
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }
}
