using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSave
{

    public static bool HasThemeData()
    {
        if (PlayerPrefs.HasKey("ThemeVar1"))
        {
            return true;
        }
        return false;
    }

    public static bool HasObstacleData()
    {
        if (PlayerPrefs.HasKey("Obstacle1"))
        {
            return true;
        }
        return false;
    }

    public static void SaveThemeData(string themeVar1, string themeVar2, string themeVar3, string themeVar4, string themeVar5)
    {
        PlayerPrefs.SetString("ThemeVar1", themeVar1);
        PlayerPrefs.SetString("ThemeVar2", themeVar2);
        PlayerPrefs.SetString("ThemeVar3", themeVar3);
        PlayerPrefs.SetString("ThemeVar4", themeVar4);
        PlayerPrefs.SetString("ThemeVar5", themeVar5);
    }

    public static void SaveObstacleData(string obstacle1, string obstacle2)
    {
        PlayerPrefs.SetString("Obstacle1", obstacle1);
        PlayerPrefs.SetString("Obstacle2", obstacle2);
    }

    public static string[] GetThemeData()
    {
        List<string> themeVars = new List<string>();
        themeVars.Add(PlayerPrefs.GetString("ThemeVar1"));
        themeVars.Add(PlayerPrefs.GetString("ThemeVar2"));
        themeVars.Add(PlayerPrefs.GetString("ThemeVar3"));
        themeVars.Add(PlayerPrefs.GetString("ThemeVar4"));
        themeVars.Add(PlayerPrefs.GetString("ThemeVar5"));
        return themeVars.ToArray();
    }

    public static string[] GetObstacleData()
    {
        List<string> obstacleVars = new List<string>();
        obstacleVars.Add(PlayerPrefs.GetString("Obstacle1"));
        obstacleVars.Add(PlayerPrefs.GetString("Obstacle2"));
        return obstacleVars.ToArray();
    }
}
