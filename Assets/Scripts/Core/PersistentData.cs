using UnityEngine;
using System.Collections.Generic;

public static class PersistentData
{
    private const string DIAMONDS_KEY = "TotalDiamonds";
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string SKILL_COUNT_PREFIX = "SkillCount_";
    private const string SELECTED_SKILLS_KEY = "SelectedSkills";

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

    public static int GetSkillCount(string skillId)
    {
        return PlayerPrefs.GetInt(SKILL_COUNT_PREFIX + skillId, 0);
    }

    public static void SetSkillCount(string skillId, int count)
    {
        PlayerPrefs.SetInt(SKILL_COUNT_PREFIX + skillId, Mathf.Max(0, count));
        PlayerPrefs.Save();
    }

    public static void AddSkill(string skillId)
    {
        int current = GetSkillCount(skillId);
        SetSkillCount(skillId, current + 1);
    }

    public static bool UseSkill(string skillId)
    {
        int current = GetSkillCount(skillId);
        if (current > 0)
        {
            SetSkillCount(skillId, current - 1);
            return true;
        }
        return false;
    }

    public static bool OwnsPowerUp(string skillId)
    {
        return GetSkillCount(skillId) > 0;
    }

    public static void AddOwnedPowerUp(string skillId)
    {
        AddSkill(skillId);
    }

    public static List<string> GetSelectedSkills()
    {
        string saved = PlayerPrefs.GetString(SELECTED_SKILLS_KEY, "");
        List<string> selected = new List<string>();
        
        if (!string.IsNullOrEmpty(saved))
        {
            string[] parts = saved.Split(',');
            foreach (string part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    selected.Add(part);
                }
            }
        }
        
        return selected;
    }

    public static void SetSelectedSkills(List<string> skills)
    {
        string combined = string.Join(",", skills.ToArray());
        PlayerPrefs.SetString(SELECTED_SKILLS_KEY, combined);
        PlayerPrefs.Save();
    }

    public static void ClearSelectedSkills()
    {
        PlayerPrefs.SetString(SELECTED_SKILLS_KEY, "");
        PlayerPrefs.Save();
    }

    public static void ConsumeSelectedSkills()
    {
        List<string> selected = GetSelectedSkills();
        foreach (string skill in selected)
        {
            UseSkill(skill);
        }
    }

    public static string GetEquippedPowerUp()
    {
        return "";
    }

    public static void SetEquippedPowerUp(string powerUpId)
    {
    }

    public static void ClearEquippedPowerUp()
    {
    }

    public static void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
