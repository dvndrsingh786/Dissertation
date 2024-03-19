using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public GameObject loadingpanel;
    public TMP_InputField inputText;
    public TMP_Text resultText;
    public List<GameObject> previewObjs;
    public Texture abcc;

    private void Awake()
    {
        instance = this;
    }


    [SerializeField] GameObject abc;
    [ContextMenu("Shooooo")]
    public void Shoo()
    {
        abc.GetComponent<Renderer>().material.mainTexture = abcc;
    }

}
