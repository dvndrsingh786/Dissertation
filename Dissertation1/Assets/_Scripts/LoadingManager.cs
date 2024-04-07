using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{

    public static LoadingManager instance;

    [Header("Loading Panel Objects")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] int loadingPanelOpeners = 0;

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
}
