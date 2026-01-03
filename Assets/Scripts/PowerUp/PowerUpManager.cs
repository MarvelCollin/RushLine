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
    private Dictionary<string, GameObject> indicatorObjects = new Dictionary<string, GameObject>();

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
        CreateIndicatorPanel();
        ActivateEquippedPowerUp();
    }

    void Update()
    {
        if (isInvincibleAfterRevive)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincibleAfterRevive = false;
                UpdateSecondChanceIndicator();
            }
        }
    }

    void CreateIndicatorPanel()
    {
        if (UIManager.Instance == null || UIManager.Instance.GetCanvas() == null) return;

        powerUpIndicatorPanel = new GameObject("PowerUpIndicators");
        powerUpIndicatorPanel.transform.SetParent(UIManager.Instance.GetCanvas().transform, false);

        RectTransform panelRect = powerUpIndicatorPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -25);
        panelRect.sizeDelta = new Vector2(500, 50);

        HorizontalLayoutGroup layout = powerUpIndicatorPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    void ActivateEquippedPowerUp()
    {
        string equipped = PersistentData.GetEquippedPowerUp();
        
        if (string.IsNullOrEmpty(equipped)) return;

        switch (equipped)
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

        PersistentData.ClearEquippedPowerUp();
    }

    void CreateIndicator(string id, string displayText, Color bgColor)
    {
        if (powerUpIndicatorPanel == null) return;
        if (indicatorObjects.ContainsKey(id)) return;

        GameObject indicator = new GameObject("Indicator_" + id);
        indicator.transform.SetParent(powerUpIndicatorPanel.transform, false);

        RectTransform rect = indicator.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(90, 40);

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
        text.fontSize = 18;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

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
        CreateIndicator("magnet", "🧲 MAGNET", new Color(0.9f, 0.4f, 0.4f, 0.9f));
    }

    public void ActivateLaser()
    {
        laserActive = true;
        CreateIndicator("laser", "🔫 LASER", new Color(1f, 0.7f, 0.2f, 0.9f));
    }

    public void ActivateTripleJump()
    {
        tripleJumpActive = true;
        CreateIndicator("triplejump", "⬆️ x3", new Color(0.4f, 0.8f, 0.4f, 0.9f));
    }

    public void ActivateDoubleDiamonds()
    {
        doubleDiamondsActive = true;
        CreateIndicator("doublediamonds", "💎 x2", new Color(0.3f, 0.7f, 0.9f, 0.9f));
    }

    public void ActivateSecondChance()
    {
        secondChanceActive = true;
        secondChanceUsed = false;
        CreateIndicator("secondchance", "💫 LIFE", new Color(0.8f, 0.5f, 0.9f, 0.9f));
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
                    text.text = "💫 " + Mathf.CeilToInt(invincibleTimer) + "s";
                    bg.color = new Color(0.3f, 0.8f, 0.3f, 0.9f);
                }
                else
                {
                    text.text = "💫 USED";
                    bg.color = new Color(0.4f, 0.4f, 0.4f, 0.9f);
                }
            }
        }
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
