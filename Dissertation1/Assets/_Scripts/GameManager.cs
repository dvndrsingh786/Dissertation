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
    public GameState gameState;
    public GameObject quadDR1;

    [Header("Panels")]
    [SerializeField] GameObject gameoverPanel;

    private void Awake()
    {
        instance = this;
        //AITools();
    }

    void AITools()
    {
        //StartCoroutine(DALLE.instance.GenerateImage("A dog flying", "1024x1024", (CoroutineReturner obj) =>
        //  {
        //      Debug.Log(obj.isSuccess);
        //      if (obj.isSuccess)
        //      {
        //          quadDR1.GetComponent<Renderer>().material.mainTexture = obj.generatedTexture;
        //      }
        //      else
        //      {
        //          Debug.Log(obj.errorMessage);
        //      }
        //  }));
        //StartCoroutine(Midjourney.instance.GenerateImageFromPrompt("A dog flying", (CoroutineReturner obj) =>
        //{
        //    Debug.Log(obj.isSuccess);
        //    if (obj.isSuccess)
        //    {
        //        quadDR1.GetComponent<Renderer>().material.mainTexture = obj.generatedTexture;
        //    }
        //    else
        //    {
        //        Debug.Log(obj.errorMessage);
        //    }
        //}));
    }

    public void GameOver()
    {
        gameoverPanel.SetActive(true);
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
    public string errorMessage = "ERROR!";
    public Texture2D generatedTexture = null;
}
