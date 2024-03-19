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

    private void Awake()
    {
        instance = this;
    }
}
