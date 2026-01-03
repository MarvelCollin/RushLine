using UnityEngine;
using System.Collections.Generic;

public static class PersistentData
{
    private const string DIAMONDS_KEY = "TotalDiamonds";
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string OWNED_POWERUPS_KEY = "OwnedPowerUps";
    private const string EQUIPPED_POWERUP_KEY = "EquippedPowerUp";

    public static int GetDiamonds()
    {
        return PlayerPrefs.GetInt(DIAMONDS_KEY, 0);
    }

    public static void SetDiamonds(int amount)
    {
        PlayerPrefs.SetInt(DIAMONDS_KEY, Mathf.Max(0, amount));
        PlayerPrefs.Save();
    }

    public static void AddDiamonds(int amount)
    {
        int current = GetDiamonds();
        SetDiamonds(current + amount);
    }

    public static bool SpendDiamonds(int amount)
    {
        int current = GetDiamonds();
        if (current >= amount)
        {
            SetDiamonds(current - amount);
            return true;
        }
        return false;
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    public static void SetHighScore(int score)
    {
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
        }
    }

    public static List<string> GetOwnedPowerUps()
    {
        string saved = PlayerPrefs.GetString(OWNED_POWERUPS_KEY, "");
        List<string> owned = new List<string>();
        
        if (!string.IsNullOrEmpty(saved))
        {
            string[] parts = saved.Split(',');
            foreach (string part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    owned.Add(part);
                }
            }
        }
        
        return owned;
    }

    public static bool OwnsPowerUp(string powerUpId)
    {
        return GetOwnedPowerUps().Contains(powerUpId);
    }

    public static void AddOwnedPowerUp(string powerUpId)
    {
        if (!OwnsPowerUp(powerUpId))
        {
            List<string> owned = GetOwnedPowerUps();
            owned.Add(powerUpId);
            string combined = string.Join(",", owned.ToArray());
            PlayerPrefs.SetString(OWNED_POWERUPS_KEY, combined);
            PlayerPrefs.Save();
        }
    }

    public static string GetEquippedPowerUp()
    {
        return PlayerPrefs.GetString(EQUIPPED_POWERUP_KEY, "");
    }

    public static void SetEquippedPowerUp(string powerUpId)
    {
        PlayerPrefs.SetString(EQUIPPED_POWERUP_KEY, powerUpId);
        PlayerPrefs.Save();
    }

    public static void ClearEquippedPowerUp()
    {
        PlayerPrefs.SetString(EQUIPPED_POWERUP_KEY, "");
        PlayerPrefs.Save();
    }

    public static void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
