using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Prompting : MonoBehaviour
{
    public static Prompting instance;
    [SerializeField] string theme = "Underwater";
    [SerializeField] string timeOfDay = "Noon";
    [SerializeField] string weatherCondition = "Sunny";
    [SerializeField] string dominantColor = "Red";
    [SerializeField] string specialFeature = "Volcano";
    [SerializeField] string obstacle1 = "apple";
    [SerializeField] string obstacle2 = "bottle";

    [SerializeField] TMP_InputField themeIP;
    [SerializeField] TMP_InputField timeOfDayIP;
    [SerializeField] TMP_InputField weatherIP;
    [SerializeField] TMP_InputField domColorIP;
    [SerializeField] TMP_InputField specialFeatureIP;
    [SerializeField] TMP_InputField obstacle1IP;
    [SerializeField] TMP_InputField obstacle2IP;

    [SerializeField] GameObject themePanel;
    [SerializeField] GameObject obstaclePanel;

    bool isThemeSet = false;
    bool isObstacleSet = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadPlayerPrefs();
    }

    void LoadPlayerPrefs()
    {
        if (PlayerSave.HasThemeData())
        {
            string[] themeData = PlayerSave.GetThemeData();
            themeIP.text = themeData[0];
            timeOfDayIP.text = themeData[1];
            weatherIP.text = themeData[2];
            domColorIP.text = themeData[3];
            specialFeatureIP.text = themeData[4];
            isThemeSet = true;
        }
        if (PlayerSave.HasObstacleData())
        {
            string[] obstacleData = PlayerSave.GetObstacleData();
            obstacle1IP.text = obstacleData[0];
            obstacle2IP.text = obstacleData[1];
            isObstacleSet = true;
        }
    }

    public bool PromptsReady()
    {
        return isThemeSet && isObstacleSet;
    }

    public string GetBGPrompt()
    {
        string prompt = $"Create an image for a 2D platformer game background. Visualise a clear setting in {theme} at {timeOfDay}, in  {weatherCondition} weather. The scene is bathed in {dominantColor} hues, reflecting the tranquil mood of {timeOfDay}. Ensure the scene includes elements like {specialFeature} to enrich the thematic depth and visual appeal. Additionally, ensure that the texture seamlessly tiles horizontally to create an endless scrolling effect in the Parallax background. Make sure there is no text in the image for an immersive visual experience.";
        return prompt;
    }

    public string[] GetObstaclesPrompt()
    {
        string[] obstacles = new string[2];
        obstacles[0] = "Generate an image of " + obstacle1 + ". Make sure there is no text or anything extra in generated image";
        obstacles[1] = "Generate an image of " + obstacle2 + ". Make sure there is no text or anything extra in generated image";
        return obstacles;
    }

    public void SetThemeFields()
    {
        theme = themeIP.text;
        timeOfDay = timeOfDayIP.text;
        weatherCondition = weatherIP.text;
        dominantColor = domColorIP.text;
        specialFeature = specialFeatureIP.text;
    }

    public void SetObstaclesFields()
    {
        obstacle1 = obstacle1IP.text;
        obstacle2 = obstacle2IP.text;
    }

    public void ThemeDoneBtn()
    {
        if(string.IsNullOrEmpty(themeIP.text) || string.IsNullOrWhiteSpace(themeIP.text))
        {
            LoadingManager.instance.ShowPopUp("Theme can't be empty");
        }
        else if (string.IsNullOrEmpty(timeOfDayIP.text) || string.IsNullOrWhiteSpace(timeOfDayIP.text))
        {
            LoadingManager.instance.ShowPopUp("Time of Day can't be empty");
        }
        else if (string.IsNullOrEmpty(weatherIP.text) || string.IsNullOrWhiteSpace(weatherIP.text))
        {
            LoadingManager.instance.ShowPopUp("Weather can't be empty");
        }
        else if (string.IsNullOrEmpty(domColorIP.text) || string.IsNullOrWhiteSpace(domColorIP.text))
        {
            LoadingManager.instance.ShowPopUp("Dominant Color can't be empty");
        }
        else if (string.IsNullOrEmpty(specialFeatureIP.text) || string.IsNullOrWhiteSpace(specialFeatureIP.text))
        {
            LoadingManager.instance.ShowPopUp("Special Feature can't be empty");
        }
        else
        {
            isThemeSet = true;
            PlayerSave.SaveThemeData(themeIP.text, timeOfDayIP.text, weatherIP.text, domColorIP.text, specialFeatureIP.text);
            themePanel.SetActive(false);
        }
    }

    public void ObstaclesDoneBtn()
    {
        if (string.IsNullOrEmpty(obstacle1IP.text) || string.IsNullOrWhiteSpace(obstacle1IP.text))
        {
            LoadingManager.instance.ShowPopUp("Obstacle 1 can't be empty");
        }
        else if (string.IsNullOrEmpty(obstacle2IP.text) || string.IsNullOrWhiteSpace(obstacle2IP.text))
        {
            LoadingManager.instance.ShowPopUp("Obstacle 2 can't be empty");
        }
        else
        {
            PlayerSave.SaveObstacleData(obstacle1IP.text, obstacle2IP.text);
            isObstacleSet = true;
            obstaclePanel.SetActive(false);
        }
    }

    public void RemoveSpaceFromIP(TMP_InputField field)
    {
        if(field.text.Contains(" "))
        {
            field.text = field.text.Replace(" ", "");
            LoadingManager.instance.ShowPopUp("Spaces not allowed");
        }
    }
}
