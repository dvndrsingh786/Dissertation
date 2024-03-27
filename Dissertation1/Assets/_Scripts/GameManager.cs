using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public TMP_InputField promptInputField;
    public TMP_InputField imageUrlField;

    [Header("Room Moving")]
    public int currentRoom = 1;
    public GameObject rightBtn;
    public GameObject leftBtn;
    public TextMeshProUGUI roomNumberTxt;

    [Header("Midjourney")]
    [SerializeField] GameObject quadMR1;
    [SerializeField] GameObject quadMR2;
    [SerializeField] GameObject quadMR3;
    [Header("DALL E")]
    [SerializeField] GameObject quadDR1;
    [SerializeField] GameObject quadDR2;
    [SerializeField] GameObject quadDR3;

    [Header("Loading Panel Objects")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] int loadingPanelOpeners = 0;

    private void Awake()
    {
        instance = this;
        rightBtn.SetActive(false);
    }

    #region Loading Panel Handler
    public void ShowLoadingPanel()
    {
        loadingPanelOpeners++;
        loadingPanel.SetActive(true);
    }

    public void HideLoadingPanel()
    {
        loadingPanelOpeners--;
        if (loadingPanelOpeners <= 0)
        {
            loadingPanelOpeners = 0;
            loadingPanel.SetActive(false);
        }
    }
    #endregion

    #region Image Processing

    public void GenerateBtnClicked()
    {
        Midjourney.instance.GenerateImageStart();
        DALLE.instance.GenerateImageStart();
    }

    public void MidjourneyImageReceived(Texture2D tex)
    {
        if (currentRoom == 1)
        {
            quadMR1.GetComponent<Renderer>().material.mainTexture = tex;
        }
        else if (currentRoom == 2)
        {
            quadMR2.GetComponent<Renderer>().material.mainTexture = tex;
        }
        else
        {
            quadMR3.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }

    public void DALLEImageReceived(Texture2D tex)
    {
        if (currentRoom == 1)
        {
            quadDR1.GetComponent<Renderer>().material.mainTexture = tex;
        }
        else if (currentRoom == 2)
        {
            quadDR2.GetComponent<Renderer>().material.mainTexture = tex;
        }
        else
        {
            quadDR3.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }

    #endregion

    public void SetRoomObjs()
    {
        if (currentRoom == 1)
        {
            rightBtn.SetActive(false);
        }
        else rightBtn.SetActive(true);
        if(currentRoom == 2)
        {
            imageUrlField.gameObject.SetActive(true);
            imageUrlField.text = "https://davdissertation.s3.eu-west-2.amazonaws.com/JoideepSir.PNG";
        }
        else
        {
            imageUrlField.gameObject.SetActive(false);
        }
        if (currentRoom == 3)
        {
            leftBtn.SetActive(false);
        }
        else leftBtn.SetActive(true);


        
        roomNumberTxt.text = "Room " + currentRoom.ToString();
    }

}
