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
        Debug.Log("[UIManager] ========== AWAKE STARTED ==========");
        Debug.Log("[UIManager] GameObject: " + gameObject.name + ", Instance null: " + (Instance == null));
        
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[UIManager] Instance set to this");
        }
        else
        {
            Debug.Log("[UIManager] Instance already exists, destroying this");
            Destroy(gameObject);
            return;
        }
        
        if (GameManager.Instance == null)
        {
            Debug.Log("[UIManager] GameManager.Instance is null, adding GameManager component");
            gameObject.AddComponent<GameManager>();
        }
        
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("[UIManager] HighScore loaded: " + highScore);
        
        CreateUI();
        CreateEventSystem();
        
        Debug.Log("[UIManager] ========== AWAKE FINISHED ==========");
    }

    void Start()
    {
        Debug.Log("[UIManager] ========== START CALLED ==========");
        Debug.Log("[UIManager] Verifying UI state in Start...");
        Debug.Log("[UIManager] canvas null: " + (canvas == null) + ", canvas enabled: " + (canvas != null && canvas.enabled));
        Debug.Log("[UIManager] scorePanel null: " + (scorePanel == null) + ", active: " + (scorePanel != null && scorePanel.activeSelf));
        Debug.Log("[UIManager] diamondPanel null: " + (diamondPanel == null) + ", active: " + (diamondPanel != null && diamondPanel.activeSelf));
        
        if (canvas != null)
        {
            Debug.Log("[UIManager] Canvas children count: " + canvas.transform.childCount);
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                Transform child = canvas.transform.GetChild(i);
                Debug.Log("[UIManager] Canvas child " + i + ": " + child.name + ", active: " + child.gameObject.activeSelf);
            }
        }
        
        Debug.Log("[UIManager] ========== CHECKING ALL CANVASES IN SCENE ==========");
        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
        Debug.Log("[UIManager] Total Canvas count in scene: " + allCanvases.Length);
        for (int i = 0; i < allCanvases.Length; i++)
        {
            Canvas c = allCanvases[i];
            Debug.Log("[UIManager] Canvas " + i + ": " + c.gameObject.name + 
                      ", RenderMode: " + c.renderMode + 
                      ", SortingOrder: " + c.sortingOrder + 
                      ", enabled: " + c.enabled + 
                      ", gameObject active: " + c.gameObject.activeInHierarchy);
        }
        
        Debug.Log("[UIManager] ========== CHECKING SCORE PANEL HIERARCHY ==========");
        if (scorePanel != null)
        {
            Debug.Log("[UIManager] ScorePanel activeInHierarchy: " + scorePanel.activeInHierarchy);
            Image img = scorePanel.GetComponent<Image>();
            if (img != null)
            {
                Debug.Log("[UIManager] ScorePanel Image enabled: " + img.enabled + ", color: " + img.color + ", raycastTarget: " + img.raycastTarget);
            }
            RectTransform rt = scorePanel.GetComponent<RectTransform>();
            if (rt != null)
            {
                Debug.Log("[UIManager] ScorePanel rect: " + rt.rect + ", localScale: " + rt.localScale);
            }
            
            Debug.Log("[UIManager] ScorePanel parent: " + (scorePanel.transform.parent != null ? scorePanel.transform.parent.name : "NULL"));
        }
        
        Debug.Log("[UIManager] ========== CHECKING DIAMOND PANEL HIERARCHY ==========");
        if (diamondPanel != null)
        {
            Debug.Log("[UIManager] DiamondPanel activeInHierarchy: " + diamondPanel.activeInHierarchy);
            Image img = diamondPanel.GetComponent<Image>();
            if (img != null)
            {
                Debug.Log("[UIManager] DiamondPanel Image enabled: " + img.enabled + ", color: " + img.color + ", raycastTarget: " + img.raycastTarget);
            }
            RectTransform rt = diamondPanel.GetComponent<RectTransform>();
            if (rt != null)
            {
                Debug.Log("[UIManager] DiamondPanel rect: " + rt.rect + ", localScale: " + rt.localScale);
            }
            
            Debug.Log("[UIManager] DiamondPanel parent: " + (diamondPanel.transform.parent != null ? diamondPanel.transform.parent.name : "NULL"));
        }
        
        Debug.Log("[UIManager] ========== FORCING CANVAS UPDATE ==========");
        Canvas.ForceUpdateCanvases();
        if (canvas != null)
        {
            CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                Debug.Log("[UIManager] Canvas has CanvasGroup: alpha=" + cg.alpha + ", interactable=" + cg.interactable + ", blocksRaycasts=" + cg.blocksRaycasts);
            }
            else
            {
                Debug.Log("[UIManager] Canvas has no CanvasGroup");
            }
        }
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
        Debug.Log("[UIManager] ========== CreateUI STARTED ==========");
        
        GameObject canvasObj = new GameObject("GameCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1f;
        canvas.sortingOrder = 999;
        
        Debug.Log("[UIManager] Canvas created - RenderMode: " + canvas.renderMode + ", SortingOrder: " + canvas.sortingOrder);
        Debug.Log("[UIManager] Canvas worldCamera: " + (canvas.worldCamera != null ? canvas.worldCamera.name : "NULL"));
        Debug.Log("[UIManager] Canvas GameObject active: " + canvasObj.activeSelf + ", Canvas enabled: " + canvas.enabled);
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        Debug.Log("[UIManager] CanvasScaler added - ScaleMode: " + scaler.uiScaleMode + ", RefRes: " + scaler.referenceResolution);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        Debug.Log("[UIManager] GraphicRaycaster added");

        CreateScorePanel();
        CreateDiamondPanel();
        CreateGameOverPanel();
        
        scorePanel.SetActive(true);
        diamondPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        
        scorePanel.transform.SetAsLastSibling();
        diamondPanel.transform.SetAsLastSibling();
        
        Debug.Log("[UIManager] ========== CreateUI VERIFICATION ==========");
        Debug.Log("[UIManager] scorePanel null: " + (scorePanel == null) + ", active: " + (scorePanel != null && scorePanel.activeSelf));
        Debug.Log("[UIManager] diamondPanel null: " + (diamondPanel == null) + ", active: " + (diamondPanel != null && diamondPanel.activeSelf));
        Debug.Log("[UIManager] scoreText null: " + (scoreText == null));
        Debug.Log("[UIManager] diamondText null: " + (diamondText == null));
        
        if (scorePanel != null)
        {
            RectTransform rt = scorePanel.GetComponent<RectTransform>();
            Debug.Log("[UIManager] ScorePanel Position: " + rt.position + ", AnchoredPos: " + rt.anchoredPosition + ", SizeDelta: " + rt.sizeDelta);
            Image img = scorePanel.GetComponent<Image>();
            Debug.Log("[UIManager] ScorePanel Image null: " + (img == null) + ", Color: " + (img != null ? img.color.ToString() : "N/A"));
        }
        
        if (diamondPanel != null)
        {
            RectTransform rt = diamondPanel.GetComponent<RectTransform>();
            Debug.Log("[UIManager] DiamondPanel Position: " + rt.position + ", AnchoredPos: " + rt.anchoredPosition + ", SizeDelta: " + rt.sizeDelta);
            Image img = diamondPanel.GetComponent<Image>();
            Debug.Log("[UIManager] DiamondPanel Image null: " + (img == null) + ", Color: " + (img != null ? img.color.ToString() : "N/A"));
        }
        
        Debug.Log("[UIManager] Screen Size: " + Screen.width + "x" + Screen.height);
        Debug.Log("[UIManager] Main Camera null: " + (Camera.main == null));
        if (Camera.main != null)
        {
            Debug.Log("[UIManager] Camera position: " + Camera.main.transform.position + ", orthographic: " + Camera.main.orthographic);
        }
        
        Debug.Log("[UIManager] ========== CreateUI FINISHED ==========");
    }

    void CreateScorePanel()
    {
        Debug.Log("[UIManager] ========== CreateScorePanel STARTED ==========");
        
        scorePanel = new GameObject("ScorePanel");
        scorePanel.transform.SetParent(canvas.transform, false);
        
        RectTransform scorePanelRect = scorePanel.AddComponent<RectTransform>();
        scorePanelRect.anchorMin = new Vector2(0f, 1f);
        scorePanelRect.anchorMax = new Vector2(0f, 1f);
        scorePanelRect.pivot = new Vector2(0f, 1f);
        scorePanelRect.anchoredPosition = new Vector2(20, -20);
        scorePanelRect.sizeDelta = new Vector2(300, 80);

        Image scoreBg = scorePanel.AddComponent<Image>();
        scoreBg.color = new Color(1f, 0f, 0f, 1f);
        
        Debug.Log("[UIManager] ScorePanel created with RectTransform and Image");
        Debug.Log("[UIManager] ScorePanel Image color: " + scoreBg.color);

        GameObject scoreLabel = new GameObject("ScoreLabel");
        scoreLabel.transform.SetParent(scorePanel.transform, false);
        RectTransform labelRect = scoreLabel.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(0.4f, 1);
        labelRect.offsetMin = new Vector2(10, 0);
        labelRect.offsetMax = Vector2.zero;
        Text labelText = scoreLabel.AddComponent<Text>();
        labelText.text = "SCORE";
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 20;
        labelText.alignment = TextAnchor.MiddleLeft;
        labelText.color = new Color(0.8f, 0.8f, 0.8f);
        
        Debug.Log("[UIManager] ScoreLabel font null: " + (labelText.font == null) + ", font name: " + (labelText.font != null ? labelText.font.name : "NULL"));

        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(scorePanel.transform, false);
        
        RectTransform scoreTextRect = scoreTextObj.AddComponent<RectTransform>();
        scoreTextRect.anchorMin = new Vector2(0.4f, 0);
        scoreTextRect.anchorMax = new Vector2(1, 1);
        scoreTextRect.offsetMin = Vector2.zero;
        scoreTextRect.offsetMax = new Vector2(-10, 0);
        
        scoreText = scoreTextObj.AddComponent<Text>();
        scoreText.text = "0";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 48;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.alignment = TextAnchor.MiddleRight;
        scoreText.color = Color.yellow;
        
        Debug.Log("[UIManager] ScoreText font null: " + (scoreText.font == null) + ", font name: " + (scoreText.font != null ? scoreText.font.name : "NULL"));
        Debug.Log("[UIManager] ScoreText text: '" + scoreText.text + "', color: " + scoreText.color + ", fontSize: " + scoreText.fontSize);

        Outline scoreOutline = scoreTextObj.AddComponent<Outline>();
        scoreOutline.effectColor = new Color(0, 0, 0, 0.8f);
        scoreOutline.effectDistance = new Vector2(2, -2);
        
        Debug.Log("[UIManager] ========== CreateScorePanel FINISHED ==========");
    }

    void CreateDiamondPanel()
    {
        Debug.Log("[UIManager] ========== CreateDiamondPanel STARTED ==========");
        
        diamondPanel = new GameObject("DiamondPanel");
        diamondPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform diamondPanelRect = diamondPanel.AddComponent<RectTransform>();
        diamondPanelRect.anchorMin = new Vector2(1f, 1f);
        diamondPanelRect.anchorMax = new Vector2(1f, 1f);
        diamondPanelRect.pivot = new Vector2(1f, 1f);
        diamondPanelRect.anchoredPosition = new Vector2(-20, -20);
        diamondPanelRect.sizeDelta = new Vector2(200, 80);

        Image diamondBg = diamondPanel.AddComponent<Image>();
        diamondBg.color = new Color(0f, 1f, 0f, 1f);
        
        Debug.Log("[UIManager] DiamondPanel created with RectTransform and Image");
        Debug.Log("[UIManager] DiamondPanel Image color: " + diamondBg.color);

        GameObject diamondIcon = new GameObject("DiamondIcon");
        diamondIcon.transform.SetParent(diamondPanel.transform, false);
        RectTransform iconRect = diamondIcon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0);
        iconRect.anchorMax = new Vector2(0.35f, 1);
        iconRect.offsetMin = new Vector2(10, 10);
        iconRect.offsetMax = new Vector2(0, -10);
        Text iconText = diamondIcon.AddComponent<Text>();
        iconText.text = "◆";
        iconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        iconText.fontSize = 32;
        iconText.alignment = TextAnchor.MiddleCenter;
        iconText.color = new Color(0.3f, 0.9f, 1f);
        
        Debug.Log("[UIManager] DiamondIcon font null: " + (iconText.font == null));

        GameObject diamondTextObj = new GameObject("DiamondText");
        diamondTextObj.transform.SetParent(diamondPanel.transform, false);
        
        RectTransform diamondTextRect = diamondTextObj.AddComponent<RectTransform>();
        diamondTextRect.anchorMin = new Vector2(0.35f, 0);
        diamondTextRect.anchorMax = new Vector2(1, 1);
        diamondTextRect.offsetMin = Vector2.zero;
        diamondTextRect.offsetMax = new Vector2(-10, 0);
        
        diamondText = diamondTextObj.AddComponent<Text>();
        diamondText.text = "0";
        diamondText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        diamondText.fontSize = 36;
        diamondText.fontStyle = FontStyle.Bold;
        diamondText.alignment = TextAnchor.MiddleRight;
        diamondText.color = Color.white;
        
        Debug.Log("[UIManager] DiamondText font null: " + (diamondText.font == null) + ", font name: " + (diamondText.font != null ? diamondText.font.name : "NULL"));
        Debug.Log("[UIManager] DiamondText text: '" + diamondText.text + "', color: " + diamondText.color + ", fontSize: " + diamondText.fontSize);

        Outline diamondOutline = diamondTextObj.AddComponent<Outline>();
        diamondOutline.effectColor = new Color(0, 0, 0, 0.8f);
        diamondOutline.effectDistance = new Vector2(2, -2);
        
        Debug.Log("[UIManager] ========== CreateDiamondPanel FINISHED ==========");
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
        modalRect.sizeDelta = new Vector2(500, 450);
        
        Image modalBg = modalBox.AddComponent<Image>();
        modalBg.color = new Color(0.15f, 0.15f, 0.2f, 1f);

        GameObject border = new GameObject("Border");
        border.transform.SetParent(modalBox.transform, false);
        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-4, -4);
        borderRect.offsetMax = new Vector2(4, 4);
        border.transform.SetAsFirstSibling();
        Image borderImg = border.AddComponent<Image>();
        borderImg.color = new Color(0.8f, 0.6f, 0.2f, 1f);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -40);
        titleRect.sizeDelta = new Vector2(400, 70);
        
        gameOverTitle = titleObj.AddComponent<Text>();
        gameOverTitle.text = "GAME OVER";
        gameOverTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        gameOverTitle.fontSize = 56;
        gameOverTitle.fontStyle = FontStyle.Bold;
        gameOverTitle.alignment = TextAnchor.MiddleCenter;
        gameOverTitle.color = new Color(0.9f, 0.3f, 0.3f, 1f);

        Outline titleOutline = titleObj.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0, 0, 0, 1f);
        titleOutline.effectDistance = new Vector2(3, -3);

        GameObject scoreObj = new GameObject("FinalScore");
        scoreObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform scoreRect = scoreObj.AddComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.5f, 1f);
        scoreRect.anchorMax = new Vector2(0.5f, 1f);
        scoreRect.pivot = new Vector2(0.5f, 1f);
        scoreRect.anchoredPosition = new Vector2(0, -130);
        scoreRect.sizeDelta = new Vector2(400, 50);
        
        finalScoreText = scoreObj.AddComponent<Text>();
        finalScoreText.text = "SCORE: 0";
        finalScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalScoreText.fontSize = 40;
        finalScoreText.fontStyle = FontStyle.Bold;
        finalScoreText.alignment = TextAnchor.MiddleCenter;
        finalScoreText.color = Color.white;

        GameObject highScoreObj = new GameObject("HighScore");
        highScoreObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform highScoreRect = highScoreObj.AddComponent<RectTransform>();
        highScoreRect.anchorMin = new Vector2(0.5f, 1f);
        highScoreRect.anchorMax = new Vector2(0.5f, 1f);
        highScoreRect.pivot = new Vector2(0.5f, 1f);
        highScoreRect.anchoredPosition = new Vector2(0, -185);
        highScoreRect.sizeDelta = new Vector2(400, 40);
        
        highScoreText = highScoreObj.AddComponent<Text>();
        highScoreText.text = "BEST: 0";
        highScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        highScoreText.fontSize = 32;
        highScoreText.alignment = TextAnchor.MiddleCenter;
        highScoreText.color = new Color(0.8f, 0.6f, 0.2f, 1f);

        GameObject finalDiamondObj = new GameObject("FinalDiamond");
        finalDiamondObj.transform.SetParent(modalBox.transform, false);
        
        RectTransform finalDiamondRect = finalDiamondObj.AddComponent<RectTransform>();
        finalDiamondRect.anchorMin = new Vector2(0.5f, 1f);
        finalDiamondRect.anchorMax = new Vector2(0.5f, 1f);
        finalDiamondRect.pivot = new Vector2(0.5f, 1f);
        finalDiamondRect.anchoredPosition = new Vector2(0, -225);
        finalDiamondRect.sizeDelta = new Vector2(400, 40);
        
        finalDiamondText = finalDiamondObj.AddComponent<Text>();
        finalDiamondText.text = "◆ 0";
        finalDiamondText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalDiamondText.fontSize = 32;
        finalDiamondText.alignment = TextAnchor.MiddleCenter;
        finalDiamondText.color = new Color(0.3f, 0.9f, 1f, 1f);

        retryButton = CreateButton(modalBox.transform, "RetryButton", "RETRY", new Vector2(0, -290), new Color(0.2f, 0.7f, 0.3f, 1f));
        retryButton.onClick.AddListener(OnRetryClicked);

        quitButton = CreateButton(modalBox.transform, "QuitButton", "QUIT", new Vector2(0, -370), new Color(0.7f, 0.2f, 0.2f, 1f));
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
        buttonRect.sizeDelta = new Vector2(280, 60);
        
        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = bgColor;
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = bgColor;
        colors.highlightedColor = bgColor * 1.2f;
        colors.pressedColor = bgColor * 0.8f;
        colors.selectedColor = bgColor;
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
        buttonText.fontSize = 36;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;

        Outline textOutline = buttonTextObj.AddComponent<Outline>();
        textOutline.effectColor = new Color(0, 0, 0, 0.5f);
        textOutline.effectDistance = new Vector2(2, -2);

        return button;
    }

    public void UpdateScore(int score)
    {
        Debug.Log("[UIManager] UpdateScore called: " + score + ", scoreText null: " + (scoreText == null));
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateDiamondCount(int count)
    {
        Debug.Log("[UIManager] UpdateDiamondCount called: " + count + ", diamondText null: " + (diamondText == null));
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
