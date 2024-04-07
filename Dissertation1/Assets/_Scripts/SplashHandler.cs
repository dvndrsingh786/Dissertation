using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SplashHandler : MonoBehaviour
{
    public Transform textsParent;
    public float textUpscaleTime = 1;
    public float splashHoldTime = 1;

    private void Start()
    {
        textsParent.DOScale(Vector3.one, textUpscaleTime).OnComplete(()=>
        {
            Invoke(nameof(LoadGameScene), splashHoldTime);
        });
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

}
