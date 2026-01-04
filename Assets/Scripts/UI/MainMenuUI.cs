using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;

    private Canvas canvas;
    private GameObject menuPanel;
    private GameObject shopPanel;
    private GameObject selectPanel;
    private Text diamondText;
    private bool isShopOpen = false;
    private System.Collections.Generic.List<string> selectedSkills = new System.Collections.Generic.List<string>();
    
    private string cheatInput = "";
    private float cheatResetTimer = 0f;
    private const string CHEAT_CODE = "kolin";
    private const float CHEAT_RESET_TIME = 2f;

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
    }

    void Start()
    {
        Time.timeScale = 0f;
        CreateCanvas();
        CreateMainMenu();
        CreateShopPanel();
        CreateSelectPanel();
        
        menuPanel.SetActive(true);
        shopPanel.SetActive(false);
        selectPanel.SetActive(false);
    }

    void Update()
    {
        HandleCheatInput();
    }

    void HandleCheatInput()
    {
        if (isShopOpen) return;

        if (cheatResetTimer > 0)
        {
            cheatResetTimer -= Time.unscaledDeltaTime;
            if (cheatResetTimer <= 0)
            {
                cheatInput = "";
            }
        }

        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                cheatInput += char.ToLower(c);
                cheatResetTimer = CHEAT_RESET_TIME;

                if (cheatInput.Length > CHEAT_CODE.Length)
                {
                    cheatInput = cheatInput.Substring(cheatInput.Length - CHEAT_CODE.Length);
                }

                if (cheatInput == CHEAT_CODE)
                {
                    PersistentData.AddDiamonds(100);
                    UpdateDiamondDisplay();
                    ShowCheatActivated();
                    cheatInput = "";
                }
            }
        }
    }

    void ShowCheatActivated()
    {
        GameObject cheatMsg = new GameObject("CheatMessage");
        cheatMsg.transform.SetParent(canvas.transform, false);

        RectTransform msgRect = cheatMsg.AddComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.pivot = new Vector2(0.5f, 0.5f);
        msgRect.anchoredPosition = new Vector2(0, -150);
        msgRect.sizeDelta = new Vector2(400, 60);

        Image msgBg = cheatMsg.AddComponent<Image>();
        msgBg.color = new Color(0.2f, 0.8f, 0.3f, 0.95f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(cheatMsg.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text msgText = textObj.AddComponent<Text>();
        msgText.text = "+100 DIAMONDS ADDED!";
        msgText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        msgText.fontSize = 28;
        msgText.fontStyle = FontStyle.Bold;
        msgText.alignment = TextAnchor.MiddleCenter;
        msgText.color = Color.white;

        Destroy(cheatMsg, 2f);
    }

    void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void CreateMainMenu()
    {
        menuPanel = new GameObject("MenuPanel");
        menuPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = menuPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = menuPanel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.1f, 0.95f);

        CreateTitle();
        CreateMenuButtons();
        CreateDiamondDisplay();
    }

    void CreateTitle()
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(menuPanel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 180);
        titleRect.sizeDelta = new Vector2(600, 120);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "RUSHLINE";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 96;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.8f, 0.2f, 1f);

        Outline outline = titleObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.8f, 0.2f, 0.1f, 1f);
        outline.effectDistance = new Vector2(4, -4);

        GameObject subtitleObj = new GameObject("Subtitle");
        subtitleObj.transform.SetParent(menuPanel.transform, false);

        RectTransform subRect = subtitleObj.AddComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.5f);
        subRect.anchorMax = new Vector2(0.5f, 0.5f);
        subRect.pivot = new Vector2(0.5f, 0.5f);
        subRect.anchoredPosition = new Vector2(0, 100);
        subRect.sizeDelta = new Vector2(400, 40);

        Text subText = subtitleObj.AddComponent<Text>();
        subText.text = "Endless Cave Runner";
        subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subText.fontSize = 28;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.7f, 0.7f, 0.8f, 1f);
    }

    void CreateMenuButtons()
    {
        float startY = 20;
        float spacing = 80;

        CreateMenuButton("PlayButton", "▶  PLAY", new Vector2(0, startY), new Color(0.2f, 0.7f, 0.3f, 1f), OnPlayClicked);
        CreateMenuButton("ShopButton", "🛒  SHOP", new Vector2(0, startY - spacing), new Color(0.3f, 0.5f, 0.9f, 1f), OnShopClicked);
        CreateMenuButton("QuitButton", "✖  QUIT", new Vector2(0, startY - spacing * 2), new Color(0.8f, 0.3f, 0.3f, 1f), OnQuitClicked);
    }

    void CreateMenuButton(string name, string text, Vector2 position, Color bgColor, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(menuPanel.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(320, 65);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = bgColor;

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        button.colors = colors;
        button.onClick.AddListener(onClick);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 36;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;

        Outline textOutline = textObj.AddComponent<Outline>();
        textOutline.effectColor = new Color(0, 0, 0, 0.5f);
        textOutline.effectDistance = new Vector2(2, -2);
    }

    void CreateDiamondDisplay()
    {
        GameObject diamondPanel = new GameObject("DiamondDisplay");
        diamondPanel.transform.SetParent(menuPanel.transform, false);

        RectTransform panelRect = diamondPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-30, -30);
        panelRect.sizeDelta = new Vector2(180, 60);

        Image bg = diamondPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(diamondPanel.transform, false);

        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(10, 0);
        iconRect.sizeDelta = new Vector2(40, 40);

        Image iconImg = iconObj.AddComponent<Image>();
        Sprite diamondSprite = Resources.Load<Sprite>("Diamond");
        if (diamondSprite != null)
        {
            iconImg.sprite = diamondSprite;
            iconImg.preserveAspect = true;
        }
        else
        {
            iconImg.color = new Color(0.3f, 0.85f, 1f, 1f);
        }

        GameObject textObj = new GameObject("DiamondText");
        textObj.transform.SetParent(diamondPanel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(55, 0);
        textRect.offsetMax = new Vector2(-10, 0);

        diamondText = textObj.AddComponent<Text>();
        diamondText.text = PersistentData.GetDiamonds().ToString();
        diamondText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        diamondText.fontSize = 32;
        diamondText.fontStyle = FontStyle.Bold;
        diamondText.alignment = TextAnchor.MiddleRight;
        diamondText.color = Color.white;
    }

    void CreateShopPanel()
    {
        shopPanel = new GameObject("ShopPanel");
        shopPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = shopPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = shopPanel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.1f, 0.98f);

        CreateShopTitle();
        CreateShopDiamondDisplay();
        CreatePowerUpCards();
        CreateBackButton();
    }

    void CreateShopTitle()
    {
        GameObject titleObj = new GameObject("ShopTitle");
        titleObj.transform.SetParent(shopPanel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -40);
        titleRect.sizeDelta = new Vector2(400, 80);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "🛒 SHOP";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 64;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.3f, 0.7f, 1f, 1f);
    }

    void CreateShopDiamondDisplay()
    {
        GameObject diamondPanel = new GameObject("ShopDiamondDisplay");
        diamondPanel.transform.SetParent(shopPanel.transform, false);

        RectTransform panelRect = diamondPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-30, -30);
        panelRect.sizeDelta = new Vector2(180, 60);

        Image bg = diamondPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(diamondPanel.transform, false);

        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(10, 0);
        iconRect.sizeDelta = new Vector2(40, 40);

        Image iconImg = iconObj.AddComponent<Image>();
        Sprite diamondSprite = Resources.Load<Sprite>("Diamond");
        if (diamondSprite != null)
        {
            iconImg.sprite = diamondSprite;
            iconImg.preserveAspect = true;
        }
        else
        {
            iconImg.color = new Color(0.3f, 0.85f, 1f, 1f);
        }

        GameObject textObj = new GameObject("ShopDiamondText");
        textObj.transform.SetParent(diamondPanel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(55, 0);
        textRect.offsetMax = new Vector2(-10, 0);

        Text text = textObj.AddComponent<Text>();
        text.text = PersistentData.GetDiamonds().ToString();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleRight;
        text.color = Color.white;
        text.name = "ShopDiamondTextComponent";
    }

    void CreatePowerUpCards()
    {
        GameObject container = new GameObject("CardsContainer");
        container.transform.SetParent(shopPanel.transform, false);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = new Vector2(0, 20);
        containerRect.sizeDelta = new Vector2(1200, 400);

        GridLayoutGroup grid = container.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(220, 320);
        grid.spacing = new Vector2(20, 20);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;

        CreatePowerUpCard(container.transform, "magnet", "🧲 MAGNET", "Attract Diamonds", 3, new Color(0.9f, 0.4f, 0.4f, 1f));
        CreatePowerUpCard(container.transform, "doublediamonds", "💎 DOUBLE", "2x Diamonds", 4, new Color(0.3f, 0.7f, 0.9f, 1f));
        CreatePowerUpCard(container.transform, "laser", "🔫 LASER", "Shoot Obstacles", 5, new Color(1f, 0.7f, 0.2f, 1f));
        CreatePowerUpCard(container.transform, "triplejump", "⬆️ TRIPLE", "3 Jumps", 3, new Color(0.4f, 0.8f, 0.4f, 1f));
        CreatePowerUpCard(container.transform, "secondchance", "💫 REVIVE", "1 Extra Life + 3s Shield", 5, new Color(0.8f, 0.5f, 0.9f, 1f));
    }

    void CreatePowerUpCard(Transform parent, string id, string title, string description, int price, Color accentColor)
    {
        GameObject card = new GameObject("Card_" + id);
        card.transform.SetParent(parent, false);

        RectTransform cardRect = card.AddComponent<RectTransform>();

        Image cardBg = card.AddComponent<Image>();
        cardBg.color = new Color(0.12f, 0.12f, 0.18f, 1f);

        GameObject accentBar = new GameObject("Accent");
        accentBar.transform.SetParent(card.transform, false);
        RectTransform accentRect = accentBar.AddComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0, 1);
        accentRect.anchorMax = new Vector2(1, 1);
        accentRect.pivot = new Vector2(0.5f, 1);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(0, 5);
        Image accentImg = accentBar.AddComponent<Image>();
        accentImg.color = accentColor;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(card.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -20);
        titleRect.sizeDelta = new Vector2(200, 60);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = title;
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 24;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;

        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(card.transform, false);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 1);
        descRect.anchorMax = new Vector2(0.5f, 1);
        descRect.pivot = new Vector2(0.5f, 1);
        descRect.anchoredPosition = new Vector2(0, -85);
        descRect.sizeDelta = new Vector2(200, 50);
        Text descText = descObj.AddComponent<Text>();
        descText.text = description;
        descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descText.fontSize = 18;
        descText.alignment = TextAnchor.MiddleCenter;
        descText.color = new Color(0.7f, 0.7f, 0.8f, 1f);

        GameObject priceObj = new GameObject("Price");
        priceObj.transform.SetParent(card.transform, false);
        RectTransform priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0.5f, 1);
        priceRect.anchorMax = new Vector2(0.5f, 1);
        priceRect.pivot = new Vector2(0.5f, 1);
        priceRect.anchoredPosition = new Vector2(0, -145);
        priceRect.sizeDelta = new Vector2(200, 40);
        Text priceText = priceObj.AddComponent<Text>();
        priceText.text = "💎 " + price.ToString();
        priceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        priceText.fontSize = 28;
        priceText.fontStyle = FontStyle.Bold;
        priceText.alignment = TextAnchor.MiddleCenter;
        priceText.color = new Color(0.3f, 0.85f, 1f, 1f);

        int ownedCount = PersistentData.GetSkillCount(id);

        GameObject ownedObj = new GameObject("Owned");
        ownedObj.transform.SetParent(card.transform, false);
        RectTransform ownedRect = ownedObj.AddComponent<RectTransform>();
        ownedRect.anchorMin = new Vector2(0.5f, 1);
        ownedRect.anchorMax = new Vector2(0.5f, 1);
        ownedRect.pivot = new Vector2(0.5f, 1);
        ownedRect.anchoredPosition = new Vector2(0, -185);
        ownedRect.sizeDelta = new Vector2(200, 30);
        Text ownedText = ownedObj.AddComponent<Text>();
        ownedText.text = "Owned: " + ownedCount.ToString();
        ownedText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ownedText.fontSize = 20;
        ownedText.fontStyle = FontStyle.Bold;
        ownedText.alignment = TextAnchor.MiddleCenter;
        ownedText.color = ownedCount > 0 ? new Color(0.4f, 0.9f, 0.4f, 1f) : new Color(0.6f, 0.6f, 0.6f, 1f);

        GameObject buttonObj = new GameObject("Button");
        buttonObj.transform.SetParent(card.transform, false);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.pivot = new Vector2(0.5f, 0);
        buttonRect.anchoredPosition = new Vector2(0, 20);
        buttonRect.sizeDelta = new Vector2(160, 50);

        Image buttonBg = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();

        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 22;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;

        buttonBg.color = new Color(0.8f, 0.6f, 0.2f, 1f);
        buttonText.text = "BUY";
        button.onClick.AddListener(() => OnBuyClicked(id, price, ownedText));
    }

    void CreateSelectPanel()
    {
        selectPanel = new GameObject("SelectPanel");
        selectPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = selectPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = selectPanel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.1f, 0.98f);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(selectPanel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -40);
        titleRect.sizeDelta = new Vector2(600, 80);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "SELECT SKILLS";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.8f, 0.2f, 1f);

        GameObject subtitleObj = new GameObject("Subtitle");
        subtitleObj.transform.SetParent(selectPanel.transform, false);
        RectTransform subRect = subtitleObj.AddComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 1f);
        subRect.anchorMax = new Vector2(0.5f, 1f);
        subRect.pivot = new Vector2(0.5f, 1f);
        subRect.anchoredPosition = new Vector2(0, -120);
        subRect.sizeDelta = new Vector2(800, 40);
        Text subText = subtitleObj.AddComponent<Text>();
        subText.text = "Click to select skills for this run (consumes 1 each)";
        subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subText.fontSize = 22;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.7f, 0.7f, 0.8f, 1f);

        CreateSelectBackButton();
        CreateStartButton();
    }

    void RefreshSelectPanel()
    {
        GameObject container = GameObject.Find("SelectCardsContainer");
        if (container != null)
        {
            Destroy(container);
        }

        container = new GameObject("SelectCardsContainer");
        container.transform.SetParent(selectPanel.transform, false);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = new Vector2(0, 0);
        containerRect.sizeDelta = new Vector2(1000, 200);

        HorizontalLayoutGroup layout = container.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        string[] skills = { "magnet", "doublediamonds", "laser", "triplejump", "secondchance" };
        string[] names = { "MAGNET", "x2 DIAMOND", "LASER", "x3 JUMP", "REVIVE" };
        Color[] colors = {
            new Color(0.9f, 0.3f, 0.3f, 1f),
            new Color(0.2f, 0.6f, 0.9f, 1f),
            new Color(1f, 0.6f, 0.1f, 1f),
            new Color(0.3f, 0.8f, 0.3f, 1f),
            new Color(0.7f, 0.4f, 0.9f, 1f)
        };

        for (int i = 0; i < skills.Length; i++)
        {
            CreateSelectCard(container.transform, skills[i], names[i], colors[i]);
        }
    }

    void CreateSelectCard(Transform parent, string id, string name, Color color)
    {
        int owned = PersistentData.GetSkillCount(id);
        bool isSelected = selectedSkills.Contains(id);

        GameObject card = new GameObject("SelectCard_" + id);
        card.transform.SetParent(parent, false);

        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(160, 180);

        LayoutElement layoutElement = card.AddComponent<LayoutElement>();
        layoutElement.minWidth = 160;
        layoutElement.minHeight = 180;
        layoutElement.preferredWidth = 160;
        layoutElement.preferredHeight = 180;

        Image cardBg = card.AddComponent<Image>();
        if (isSelected)
        {
            cardBg.color = color;
        }
        else if (owned > 0)
        {
            cardBg.color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 0.9f);
        }
        else
        {
            cardBg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);
        }

        Button cardButton = card.AddComponent<Button>();
        string skillId = id;
        cardButton.onClick.AddListener(() => OnSelectSkillClicked(skillId));

        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(card.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.5f, 0.5f);
        nameRect.anchorMax = new Vector2(0.5f, 0.5f);
        nameRect.pivot = new Vector2(0.5f, 0.5f);
        nameRect.anchoredPosition = new Vector2(0, 20);
        nameRect.sizeDelta = new Vector2(150, 50);
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = name;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 18;
        nameText.fontStyle = FontStyle.Bold;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.white;

        GameObject ownedObj = new GameObject("Owned");
        ownedObj.transform.SetParent(card.transform, false);
        RectTransform ownedRect = ownedObj.AddComponent<RectTransform>();
        ownedRect.anchorMin = new Vector2(0.5f, 0f);
        ownedRect.anchorMax = new Vector2(0.5f, 0f);
        ownedRect.pivot = new Vector2(0.5f, 0f);
        ownedRect.anchoredPosition = new Vector2(0, 40);
        ownedRect.sizeDelta = new Vector2(150, 30);
        Text ownedText = ownedObj.AddComponent<Text>();
        ownedText.text = "x" + owned.ToString();
        ownedText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ownedText.fontSize = 24;
        ownedText.fontStyle = FontStyle.Bold;
        ownedText.alignment = TextAnchor.MiddleCenter;
        ownedText.color = owned > 0 ? new Color(0.4f, 1f, 0.4f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);

        GameObject statusObj = new GameObject("Status");
        statusObj.transform.SetParent(card.transform, false);
        RectTransform statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0f);
        statusRect.anchorMax = new Vector2(0.5f, 0f);
        statusRect.pivot = new Vector2(0.5f, 0f);
        statusRect.anchoredPosition = new Vector2(0, 15);
        statusRect.sizeDelta = new Vector2(150, 25);
        Text statusText = statusObj.AddComponent<Text>();
        if (owned == 0)
        {
            statusText.text = "BUY IN SHOP";
            statusText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
        else if (isSelected)
        {
            statusText.text = "SELECTED";
            statusText.color = new Color(1f, 1f, 0.3f, 1f);
        }
        else
        {
            statusText.text = "TAP TO SELECT";
            statusText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 12;
        statusText.alignment = TextAnchor.MiddleCenter;
    }

    void OnSelectSkillClicked(string id)
    {
        int owned = PersistentData.GetSkillCount(id);
        if (owned == 0) return;

        if (selectedSkills.Contains(id))
        {
            selectedSkills.Remove(id);
        }
        else
        {
            selectedSkills.Add(id);
        }
        RefreshSelectPanel();
    }

    void CreateSelectBackButton()
    {
        GameObject buttonObj = new GameObject("SelectBackButton");
        buttonObj.transform.SetParent(selectPanel.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 1);
        buttonRect.anchorMax = new Vector2(0, 1);
        buttonRect.pivot = new Vector2(0, 1);
        buttonRect.anchoredPosition = new Vector2(30, -30);
        buttonRect.sizeDelta = new Vector2(120, 50);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.5f, 0.5f, 0.6f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(OnSelectBackClicked);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = "← BACK";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 20;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
    }

    void CreateStartButton()
    {
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(selectPanel.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = new Vector2(0, 50);
        buttonRect.sizeDelta = new Vector2(250, 70);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.2f, 0.8f, 0.3f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(OnStartGameClicked);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = "▶ START GAME";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 28;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
    }

    void OnSelectBackClicked()
    {
        selectPanel.SetActive(false);
        menuPanel.SetActive(true);
    }


    void CreateBackButton()
    {
        GameObject buttonObj = new GameObject("BackButton");
        buttonObj.transform.SetParent(shopPanel.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 1);
        buttonRect.anchorMax = new Vector2(0, 1);
        buttonRect.pivot = new Vector2(0, 1);
        buttonRect.anchoredPosition = new Vector2(30, -30);
        buttonRect.sizeDelta = new Vector2(120, 50);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.5f, 0.5f, 0.6f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(OnBackClicked);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = "← BACK";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 24;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
    }

    void OnPlayClicked()
    {
        menuPanel.SetActive(false);
        selectedSkills.Clear();
        RefreshSelectPanel();
        selectPanel.SetActive(true);
    }

    void OnStartGameClicked()
    {
        PersistentData.SetSelectedSkills(selectedSkills);
        Time.timeScale = 1f;
        Destroy(canvas.gameObject);
        Destroy(gameObject);
    }

    void OnShopClicked()
    {
        menuPanel.SetActive(false);
        
        Destroy(shopPanel);
        CreateShopPanel();
        shopPanel.SetActive(true);
        
        isShopOpen = true;
    }

    void OnQuitClicked()
    {
        Application.Quit();
    }

    void OnBackClicked()
    {
        shopPanel.SetActive(false);
        menuPanel.SetActive(true);
        UpdateDiamondDisplay();
        isShopOpen = false;
    }

    void OnBuyClicked(string id, int price, Text ownedText)
    {
        if (PersistentData.SpendDiamonds(price))
        {
            PersistentData.AddOwnedPowerUp(id);
            
            int newCount = PersistentData.GetSkillCount(id);
            ownedText.text = "Owned: " + newCount.ToString();
            ownedText.color = new Color(0.4f, 0.9f, 0.4f, 1f);

            UpdateShopDiamondDisplay();
            UpdateDiamondDisplay();
        }
    }

    void OnEquipClicked(string id, Text buttonText, Image buttonBg)
    {
        string previousEquipped = PersistentData.GetEquippedPowerUp();
        
        PersistentData.SetEquippedPowerUp(id);
        
        buttonText.text = "EQUIPPED";
        buttonBg.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        
        buttonText.GetComponentInParent<Button>().onClick.RemoveAllListeners();
        buttonText.GetComponentInParent<Button>().onClick.AddListener(() => OnUnequipClicked(id, buttonText, buttonBg));

        if (!string.IsNullOrEmpty(previousEquipped) && previousEquipped != id)
        {
            RefreshShopPanel();
        }
    }

    void OnUnequipClicked(string id, Text buttonText, Image buttonBg)
    {
        PersistentData.ClearEquippedPowerUp();
        
        buttonText.text = "EQUIP";
        buttonBg.color = new Color(0.3f, 0.5f, 0.8f, 1f);
        
        buttonText.GetComponentInParent<Button>().onClick.RemoveAllListeners();
        buttonText.GetComponentInParent<Button>().onClick.AddListener(() => OnEquipClicked(id, buttonText, buttonBg));
    }

    void RefreshShopPanel()
    {
        shopPanel.SetActive(false);
        Destroy(shopPanel);
        CreateShopPanel();
        shopPanel.SetActive(true);
    }

    void UpdateDiamondDisplay()
    {
        if (diamondText != null)
        {
            diamondText.text = PersistentData.GetDiamonds().ToString();
        }
    }

    void UpdateShopDiamondDisplay()
    {
        GameObject shopDiamondText = GameObject.Find("ShopDiamondText");
        if (shopDiamondText != null)
        {
            Text text = shopDiamondText.GetComponent<Text>();
            if (text != null)
            {
                text.text = PersistentData.GetDiamonds().ToString();
            }
        }
        
        Text[] allTexts = shopPanel.GetComponentsInChildren<Text>();
        foreach (Text t in allTexts)
        {
            if (t.gameObject.name == "ShopDiamondText" || t.name == "ShopDiamondTextComponent")
            {
                t.text = PersistentData.GetDiamonds().ToString();
            }
        }
    }
}
