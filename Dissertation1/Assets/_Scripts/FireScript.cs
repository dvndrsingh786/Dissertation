using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{

    SpriteRenderer sr;
    [SerializeField] Sprite[] fireSprites;
    [SerializeField] float fireSpeed;
    [SerializeField] int currentFireIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Invoke(nameof(ChangeFire), 1 / fireSpeed);
    }

    void ChangeFire()
    {
        currentFireIndex++;
        if (currentFireIndex >= fireSprites.Length) currentFireIndex = 0;
        sr.sprite = fireSprites[currentFireIndex];
        Invoke(nameof(ChangeFire), 1 / fireSpeed);
    }

}
