using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingManager : MonoBehaviour
{

    public static LoadingManager instance;

    [Header("Loading Panel Objects")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] int loadingPanelOpeners = 0;
    [SerializeField] TextMeshProUGUI loadingCaptionText;

    [SerializeField] GameObject popUpPanel;
    [SerializeField] TextMeshProUGUI popUpTxt;

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

    #region Loading Panel Handler
    public void ShowLoadingPanel()
    {
        loadingPanelOpeners++;
        loadingPanel.SetActive(true);
    }

    public void AddCaptionToLoadingPanel(string loadingCaption)
    {
        loadingCaptionText.text += loadingCaption;
    }

    public void HideLoadingPanel()
    {
        loadingPanelOpeners--;
        if (loadingPanelOpeners <= 0)
        {
            loadingCaptionText.text = "";
            loadingPanelOpeners = 0;
            loadingPanel.SetActive(false);
        }
    }
    #endregion

    public void ShowPopUp(string popUpMsg, float popUpTime=2f)
    {
        popUpTxt.text = popUpMsg;
        popUpPanel.SetActive(true);
        Invoke(nameof(HidePopUp), popUpTime);
    }

    void HidePopUp()
    {
        popUpPanel.SetActive(false);
    }
}
