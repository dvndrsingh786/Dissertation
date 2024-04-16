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
    [SerializeField] SpriteRenderer abc;

    public int currentAIIndex = 0;
    public Sprite[] dallEImages;
    [SerializeField] int dallEImagesCount = 0;
    public Sprite[] midjourneyImages;
    [SerializeField] int midjourneyImagesCount = 0;

    [Header("Panels & UI")]
    public GameObject gameoverPanel;
    public GameObject menuPanel;
    public GameObject gameplayUI;
    public GameObject resultPanel;
    public Image dallEResultImg;
    public Image midjourneyResultImg;
    public GameObject nextBtn;
    public GameObject resultsBtn;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        StartCoroutine(DALLE.instance.GenerateImage("", "1024x1024", (obj) =>
        {
            if (obj.isSuccess)
            {
                Debug.Log("Generated Texture");
                //DALLE.instance.WriteImageOnDisk(obj.generatedTexture, "DAVVV");
                dallEImages[0] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 1024, 1024), Vector2.one * 0.5f);
            }
            else
            {
                Debug.Log(obj.errorMessage);
            }
        }));
    }

    #region Color Remover
    void RemoveBackground(Texture2D input, Texture2D output, Color backgroundColor, float threshold)
    {
        for (int x = 0; x < input.width; x++)
        {
            for (int y = 0; y < input.height; y++)
            {
                Color pixelColor = input.GetPixel(x, y);
                if (ColorSimilarity(pixelColor, backgroundColor) <= threshold) // Check if colors are similar within threshold
                    pixelColor.a = 0; // Set alpha to 0 to make it transparent
                output.SetPixel(x, y, pixelColor);
            }
        }
        output.Apply();
    }

    float ColorSimilarity(Color color1, Color color2)
    {
        float rDiff = Mathf.Abs(color1.r - color2.r);
        float gDiff = Mathf.Abs(color1.g - color2.g);
        float bDiff = Mathf.Abs(color1.b - color2.b);
        return (rDiff + gDiff + bDiff) / 3; // Average difference
    }
    #endregion

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (currentAIIndex == 1)
        {
            Debug.Log("Midjourney Time");
            StartCoroutine(CheckMidjourneyImages1());
            //CheckMidjourneyImages();
        }
    }

    IEnumerator CheckMidjourneyImages1()
    {
        LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 1 of 3");
        yield return new WaitForSeconds(3);
        LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 2 of 3");
        yield return new WaitForSeconds(3);
        LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 3 of 3");
        yield return new WaitForSeconds(3);
        StartGame();
    }

    void CheckMidjourneyImages()
    {
        if (midjourneyImages[0] == null)
        {
            LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 1 of 3");
            Invoke(nameof(CheckMidjourneyImages), 3);
        }
        else if (midjourneyImages[1] == null)
        {
            LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 2 of 3");
            Invoke(nameof(CheckMidjourneyImages), 3);
        }
        else if (midjourneyImages[1] == null)
        {
            LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 3 of 3");
            Invoke(nameof(CheckMidjourneyImages), 3);
        }
        else
        {
            StartGame();
        }
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
                  else if (dallEImagesCount == 1)
                  {
                      dallEImages[dallEImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 512, 512), Vector2.one * 0.5f);
                      StartGame();
                  }
                  //else
                  //{
                  //    dallEImages[dallEImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 512, 512), Vector2.one * 0.5f);
                  //    StartGame();
                  //}
                  dallEImagesCount++;
                  if (dallEImagesCount == 1)
                  {
                      GetImageFromDallE(Prompting.instance.GetObstaclesPrompt()[0]);
                  }
                  //else if (dallEImagesCount == 2)
                  //{
                  //    GetImageFromDallE(Prompting.instance.GetObstaclesPrompt()[1]);
                  //}
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
                    LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 2 of 3");
                }
                else if (midjourneyImagesCount == 1)
                {
                    midjourneyImages[midjourneyImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 512, 512), Vector2.one * 0.5f);
                    LoadingManager.instance.SetCaptionOfLoadingPanel("Fetching Image 2 of 3");
                }
                //else
                //{
                //    midjourneyImages[midjourneyImagesCount] = Sprite.Create(obj.generatedTexture, new Rect(0, 0, 512, 512), Vector2.one * 0.5f);
                //}
                midjourneyImagesCount++;
                if (midjourneyImagesCount == 1)
                {
                    GetImageFromMidjourney(Prompting.instance.GetObstaclesPrompt()[0]);
                }
                //else if (midjourneyImagesCount == 2)
                //{
                //    GetImageFromMidjourney(Prompting.instance.GetObstaclesPrompt()[1]);
                //}
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
                    //StartGame();
                    GetImageFromMidjourney(Prompting.instance.GetBGPrompt());
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
        nextBtn.SetActive(currentAIIndex == 0);
        resultsBtn.SetActive(currentAIIndex == 1);
        gameoverPanel.SetActive(true);
        gameplayUI.SetActive(false);
        gameState = GameState.GameEnd;
    }

    public void StartGameWithAI2()
    {
        LoadingManager.instance.ShowLoadingPanel();
        currentAIIndex = 1;
        foreach (var item in GameplayManager.instance.backgrounds)
        {
            item.GetComponent<SpriteRenderer>().sprite = midjourneyImages[0];
        }
        SceneManager.LoadScene(1);
    }

    public void ShowResultScreen()
    {

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
