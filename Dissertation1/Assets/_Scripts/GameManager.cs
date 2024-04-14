using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    MenuScreen,
    Gameplay,
    GameEnd
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameState gameState;
    public GameObject quadDR1;
    [SerializeField]SpriteRenderer abc;

    public int currentAIIndex = 0;
    public Sprite[] dallEImages;
    [SerializeField] int dallEImagesCount = 0;
    public Sprite[] midjourneyImages;
    [SerializeField] int midjourneyImagesCount = 0;
    [Header("Panels & UI")]
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gameplayUI;

    private void Awake()
    {
        instance = this;
    }

    void GetImageFromDallE(string _prompt)
    {
        string res = "";
        if (dallEImagesCount == 0) res = "1792x1024";
        else res = "1024x1024";
        StartCoroutine(DALLE.instance.GenerateImage(_prompt, res, (CoroutineReturner obj) =>
          {
              Debug.Log(obj.isSuccess);
              if (obj.isSuccess)
              {
                  //quadDR1.GetComponent<Renderer>().material.mainTexture = obj.generatedTexture;
                  //abc.sprite = Sprite.Create(obj.generatedTexture, new Rect(0, 0, obj.generatedTexture.width, obj.generatedTexture.height), Vector2.one * 0.5f);
                  Debug.Log(dallEImagesCount);
                  if (dallEImagesCount == 0)
                  {
                      dallEImages[dallEImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 1792, 1024), Vector2.one * 0.5f);
                      foreach (var item in GameplayManager.instance.backgrounds)
                      {
                          item.GetComponent<SpriteRenderer>().sprite = dallEImages[dallEImagesCount];
                      }
                  }
                  else
                  {
                      dallEImages[dallEImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 1024, 1024), Vector2.one * 0.5f);
                      StartGame();
                  }
                  dallEImagesCount++;
                  if (dallEImagesCount == 1)
                  {
                      GetImageFromDallE(Prompting.instance.GetObstaclesPrompt()[0]);
                  }
              }
              else
              {
                  Debug.Log(obj.errorMessage);
                  LoadingManager.instance.ShowPopUp(obj.errorMessage);
                  LoadingManager.instance.HideLoadingPanel();
              }
          }));
    }

    void GetImageFromMidjourney(string _prompt)
    {
        string asRatio = "";
        if (midjourneyImagesCount == 0) asRatio = "7:4";
        else asRatio = "1:1";
        StartCoroutine(Midjourney.instance.GenerateImageFromPrompt(_prompt, asRatio, (CoroutineReturner obj) =>
        {
            Debug.Log(obj.isSuccess);
            if (obj.isSuccess)
            {
                //quadDR1.GetComponent<Renderer>().material.mainTexture = obj.generatedTexture;
                if (midjourneyImagesCount == 0)
                {
                    midjourneyImages[midjourneyImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 1792, 1024), Vector2.one * 0.5f);
                }
                else
                {
                    midjourneyImages[midjourneyImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 1024, 1024), Vector2.one * 0.5f);
                }
                midjourneyImagesCount++;
                if (midjourneyImagesCount == 1)
                {
                    GetImageFromMidjourney(Prompting.instance.GetObstaclesPrompt()[0]);
                }
                else
                {

                }
            }
            else
            {
                Debug.Log(obj.errorMessage);
                LoadingManager.instance.ShowPopUp(obj.errorMessage);
                LoadingManager.instance.HideLoadingPanel();
            }
        }));
    }

    public void PlayBtnClicked()
    {
        if (!Prompting.instance.PromptsReady())
        {
            Debug.Log("Not Ready to start");
            LoadingManager.instance.ShowPopUp("Select Theme or Obstacles first");
            return;
        }
        string temp = Prompting.instance.GetBGPrompt() + " " + Prompting.instance.GetObstaclesPrompt()[0] + " " + Prompting.instance.GetObstaclesPrompt()[1];
        LoadingManager.instance.ShowLoadingPanel();
        DALLE.instance.ValidateContent(temp,(obj) =>
        {
            if (obj.isSuccess)
            {
                if (obj.isValidContent)
                {
                    Debug.Log("Valid Content");
                    GetImageFromDallE(Prompting.instance.GetBGPrompt());
                    //GetImageFromMidjourney(Prompting.instance.GetBGPrompt());
                }
                else
                {
                    Debug.Log("Not Valid Content");
                    LoadingManager.instance.ShowPopUp(obj.successMsg);
                    LoadingManager.instance.HideLoadingPanel();
                }
            }
            else
            {
                LoadingManager.instance.ShowPopUp(obj.errorMessage);
                LoadingManager.instance.HideLoadingPanel();
                Debug.Log("Failed");
            }
        });

    }

    void StartGame()
    {
        Debug.Log("Starting game");
        gameplayUI.SetActive(true);
        menuPanel.SetActive(false);
        gameState = GameState.Gameplay;
        LoadingManager.instance.HideLoadingPanel();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        gameoverPanel.SetActive(true);
        gameplayUI.SetActive(false);
        gameState = GameState.GameEnd;
    }

    public void MainMenuPanel()
    {
        SceneManager.LoadScene(1);
    }
}

[System.Serializable]
public class CoroutineReturner
{
    public bool isSuccess = false;
    public bool isValidContent = false;
    public string errorMessage = "ERROR!";
    public string successMsg = "";
    public Texture2D generatedTexture = null;
}
