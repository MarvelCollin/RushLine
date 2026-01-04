using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    private bool magnetActive = false;
    private bool laserActive = false;
    private bool tripleJumpActive = false;
    private bool doubleDiamondsActive = false;
    private bool secondChanceActive = false;
    private bool secondChanceUsed = false;
    
    private bool isInvincibleAfterRevive = false;
    private float invincibleTimer = 0f;
    private const float REVIVE_INVINCIBLE_DURATION = 3f;

    private GameObject powerUpIndicatorPanel;
    private GameObject powerUpHotbarPanel;
    private Dictionary<string, GameObject> indicatorObjects = new Dictionary<string, GameObject>();
    
    private bool uiInitialized = false;
    private bool equippedActivated = false;

    private string[] powerUpOrder = { "magnet", "doublediamonds", "laser", "triplejump", "secondchance" };
    private string[] powerUpNames = { "MAGNET", "x2 GEM", "LASER", "JUMP", "REVIVE" };
    private Color[] powerUpColors = {
        new Color(0.9f, 0.3f, 0.3f, 1f),
        new Color(0.2f, 0.6f, 0.9f, 1f),
        new Color(1f, 0.6f, 0.1f, 1f),
        new Color(0.3f, 0.8f, 0.3f, 1f),
        new Color(0.7f, 0.4f, 0.9f, 1f)
    };

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

    void Update()
    {
        if (!uiInitialized && UIManager.Instance != null && UIManager.Instance.GetCanvas() != null)
        {
            CreateIndicatorPanel();
            CreateHotbarPanel();
            uiInitialized = true;
        }

        if (uiInitialized && !equippedActivated && GameManager.Instance != null && GameManager.Instance.GetCurrentState() == GameState.Playing)
        {
            ActivateEquippedPowerUp();
            equippedActivated = true;
        }

        if (isInvincibleAfterRevive)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincibleAfterRevive = false;
                UpdateSecondChanceIndicator();
            }
        }

        HandleHotkeyInput();
    }

    void HandleHotkeyInput()
    {
        if (GameManager.Instance == null || GameManager.Instance.GetCurrentState() != GameState.Playing) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            TryActivatePowerUp("magnet");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            TryActivatePowerUp("doublediamonds");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            TryActivatePowerUp("laser");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            TryActivatePowerUp("triplejump");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            TryActivatePowerUp("secondchance");
        }
    }

    void TryActivatePowerUp(string id)
    {
        if (!PersistentData.OwnsPowerUp(id)) return;

        bool alreadyActive = false;
        switch (id)
        {
            case "magnet": alreadyActive = magnetActive; break;
            case "doublediamonds": alreadyActive = doubleDiamondsActive; break;
            case "laser": alreadyActive = laserActive; break;
            case "triplejump": alreadyActive = tripleJumpActive; break;
            case "secondchance": alreadyActive = secondChanceActive; break;
        }

        if (id == "laser" && alreadyActive)
        {
            DeactivateLaser();
            UpdateHotbar();
            return;
        }

        if (alreadyActive) return;

        switch (id)
        {
            case "magnet": ActivateMagnet(); break;
            case "doublediamonds": ActivateDoubleDiamonds(); break;
            case "laser": ActivateLaser(); break;
            case "triplejump": ActivateTripleJump(); break;
            case "secondchance": ActivateSecondChance(); break;
        }

        UpdateHotbar();
    }

    void CreateIndicatorPanel()
    {
        if (UIManager.Instance == null || UIManager.Instance.GetCanvas() == null) return;

        powerUpIndicatorPanel = new GameObject("PowerUpIndicators");
        powerUpIndicatorPanel.transform.SetParent(UIManager.Instance.GetCanvas().transform, false);

        RectTransform panelRect = powerUpIndicatorPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(20, -80);
        panelRect.sizeDelta = new Vector2(600, 45);

        HorizontalLayoutGroup layout = powerUpIndicatorPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    void CreateHotbarPanel()
    {
        if (UIManager.Instance == null || UIManager.Instance.GetCanvas() == null) return;

        powerUpHotbarPanel = new GameObject("PowerUpHotbar");
        powerUpHotbarPanel.transform.SetParent(UIManager.Instance.GetCanvas().transform, false);

        RectTransform panelRect = powerUpHotbarPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0f, 0f);
        panelRect.pivot = new Vector2(0f, 0f);
        panelRect.anchoredPosition = new Vector2(20, 20);
        panelRect.sizeDelta = new Vector2(520, 80);

        Image panelBg = powerUpHotbarPanel.AddComponent<Image>();
        panelBg.color = new Color(0.08f, 0.08f, 0.12f, 0.9f);

        HorizontalLayoutGroup layout = powerUpHotbarPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        for (int i = 0; i < powerUpOrder.Length; i++)
        {
            CreateHotbarSlot(i);
        }
    }

    void CreateHotbarSlot(int index)
    {
        string id = powerUpOrder[index];
        bool owned = PersistentData.OwnsPowerUp(id);
        bool active = IsActiveById(id);

        GameObject slot = new GameObject("Slot_" + id);
        slot.transform.SetParent(powerUpHotbarPanel.transform, false);

        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(90, 56);

        LayoutElement layoutElement = slot.AddComponent<LayoutElement>();
        layoutElement.minWidth = 90;
        layoutElement.minHeight = 56;
        layoutElement.preferredWidth = 90;
        layoutElement.preferredHeight = 56;

        Image slotBg = slot.AddComponent<Image>();
        if (active)
        {
            slotBg.color = powerUpColors[index];
        }
        else if (owned)
        {
            slotBg.color = new Color(powerUpColors[index].r * 0.5f, powerUpColors[index].g * 0.5f, powerUpColors[index].b * 0.5f, 0.9f);
        }
        else
        {
            slotBg.color = new Color(0.2f, 0.2f, 0.25f, 0.8f);
        }

        GameObject keyLabel = new GameObject("KeyLabel");
        keyLabel.transform.SetParent(slot.transform, false);
        RectTransform keyRect = keyLabel.AddComponent<RectTransform>();
        keyRect.anchorMin = new Vector2(0f, 1f);
        keyRect.anchorMax = new Vector2(0f, 1f);
        keyRect.pivot = new Vector2(0f, 1f);
        keyRect.anchoredPosition = new Vector2(4, -4);
        keyRect.sizeDelta = new Vector2(20, 18);

        Image keyBg = keyLabel.AddComponent<Image>();
        keyBg.color = owned ? new Color(1f, 1f, 1f, 0.95f) : new Color(0.3f, 0.3f, 0.3f, 0.7f);

        GameObject keyText = new GameObject("KeyText");
        keyText.transform.SetParent(keyLabel.transform, false);
        RectTransform keyTextRect = keyText.AddComponent<RectTransform>();
        keyTextRect.anchorMin = Vector2.zero;
        keyTextRect.anchorMax = Vector2.one;
        keyTextRect.offsetMin = Vector2.zero;
        keyTextRect.offsetMax = Vector2.zero;

        Text keyTextComp = keyText.AddComponent<Text>();
        keyTextComp.text = (index + 1).ToString();
        keyTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        keyTextComp.fontSize = 12;
        keyTextComp.fontStyle = FontStyle.Bold;
        keyTextComp.alignment = TextAnchor.MiddleCenter;
        keyTextComp.color = owned ? Color.black : new Color(0.5f, 0.5f, 0.5f, 1f);

        GameObject nameLabel = new GameObject("NameLabel");
        nameLabel.transform.SetParent(slot.transform, false);
        RectTransform nameRect = nameLabel.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.5f, 0f);
        nameRect.anchorMax = new Vector2(0.5f, 0f);
        nameRect.pivot = new Vector2(0.5f, 0f);
        nameRect.anchoredPosition = new Vector2(0, 8);
        nameRect.sizeDelta = new Vector2(85, 32);

        Text nameText = nameLabel.AddComponent<Text>();
        bool isToggleable = (id == "laser");
        
        if (active)
        {
            if (isToggleable)
            {
                nameText.text = powerUpNames[index] + " [ON]";
            }
            else
            {
                nameText.text = powerUpNames[index];
            }
        }
        else if (owned)
        {
            if (isToggleable)
            {
                nameText.text = powerUpNames[index] + " [OFF]";
            }
            else
            {
                nameText.text = powerUpNames[index];
            }
        }
        else
        {
            nameText.text = "LOCKED";
        }
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 10;
        nameText.fontStyle = FontStyle.Bold;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = active ? new Color(1f, 1f, 0.3f, 1f) : (owned ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f));
    }

    public void UpdateHotbar()
    {
        if (powerUpHotbarPanel == null) return;

        foreach (Transform child in powerUpHotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < powerUpOrder.Length; i++)
        {
            CreateHotbarSlot(i);
        }
    }

    bool IsActiveById(string id)
    {
        switch (id)
        {
            case "magnet": return magnetActive;
            case "doublediamonds": return doubleDiamondsActive;
            case "laser": return laserActive;
            case "triplejump": return tripleJumpActive;
            case "secondchance": return secondChanceActive;
            default: return false;
        }
    }

    void ActivateEquippedPowerUp()
    {
        System.Collections.Generic.List<string> selected = PersistentData.GetSelectedSkills();
        
        if (selected.Count == 0) return;

        foreach (string skillId in selected)
        {
            switch (skillId)
            {
                case "magnet":
                    ActivateMagnet();
                    break;
                case "laser":
                    ActivateLaser();
                    break;
                case "triplejump":
                    ActivateTripleJump();
                    break;
                case "doublediamonds":
                    ActivateDoubleDiamonds();
                    break;
                case "secondchance":
                    ActivateSecondChance();
                    break;
            }
        }

        PersistentData.ConsumeSelectedSkills();
        PersistentData.ClearSelectedSkills();
        UpdateHotbar();
    }

    void CreateIndicator(string id, string displayText, Color bgColor)
    {
        if (powerUpIndicatorPanel == null) return;
        if (indicatorObjects.ContainsKey(id)) return;

        GameObject indicator = new GameObject("Indicator_" + id);
        indicator.transform.SetParent(powerUpIndicatorPanel.transform, false);

        RectTransform rect = indicator.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(85, 35);

        Image bg = indicator.AddComponent<Image>();
        bg.color = bgColor;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(indicator.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = displayText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(1, -1);

        indicatorObjects[id] = indicator;
    }

    void RemoveIndicator(string id)
    {
        if (indicatorObjects.ContainsKey(id))
        {
            if (indicatorObjects[id] != null)
            {
                Destroy(indicatorObjects[id]);
            }
            indicatorObjects.Remove(id);
        }
    }

    public void ActivateMagnet()
    {
        magnetActive = true;
        CreateIndicator("magnet", "MAGNET", powerUpColors[0]);
    }

    public void ActivateLaser()
    {
        laserActive = true;
        CreateIndicator("laser", "LASER ON", powerUpColors[2]);
    }

    public void DeactivateLaser()
    {
        laserActive = false;
        RemoveIndicator("laser");
    }

    public void ActivateTripleJump()
    {
        tripleJumpActive = true;
        CreateIndicator("triplejump", "x3 JUMP", powerUpColors[3]);
    }

    public void ActivateDoubleDiamonds()
    {
        doubleDiamondsActive = true;
        CreateIndicator("doublediamonds", "x2 GEM", powerUpColors[1]);
    }

    public void ActivateSecondChance()
    {
        secondChanceActive = true;
        secondChanceUsed = false;
        CreateIndicator("secondchance", "REVIVE", powerUpColors[4]);
    }

    void UpdateSecondChanceIndicator()
    {
        if (indicatorObjects.ContainsKey("secondchance") && indicatorObjects["secondchance"] != null)
        {
            Text text = indicatorObjects["secondchance"].GetComponentInChildren<Text>();
            Image bg = indicatorObjects["secondchance"].GetComponent<Image>();
            
            if (secondChanceUsed)
            {
                if (isInvincibleAfterRevive)
                {
                    text.text = Mathf.CeilToInt(invincibleTimer) + "s";
                    bg.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                }
                else
                {
                    text.text = "USED";
                    bg.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
                }
            }
        }
        UpdateHotbar();
    }

    public bool TryUseSecondChance()
    {
        if (secondChanceActive && !secondChanceUsed)
        {
            secondChanceUsed = true;
            isInvincibleAfterRevive = true;
            invincibleTimer = REVIVE_INVINCIBLE_DURATION;
            UpdateSecondChanceIndicator();
            return true;
        }
        return false;
    }

    public bool IsMagnetActive()
    {
        return magnetActive;
    }

    public bool IsLaserActive()
    {
        return laserActive;
    }

    public bool IsTripleJumpActive()
    {
        return tripleJumpActive;
    }

    public bool IsDoubleDiamondsActive()
    {
        return doubleDiamondsActive;
    }

    public bool IsSecondChanceAvailable()
    {
        return secondChanceActive && !secondChanceUsed;
    }

    public bool IsInvincibleAfterRevive()
    {
        return isInvincibleAfterRevive;
    }

    public int GetMaxJumps()
    {
        return tripleJumpActive ? 3 : 2;
    }

    public int GetDiamondMultiplier()
    {
        return doubleDiamondsActive ? 2 : 1;
    }
}
