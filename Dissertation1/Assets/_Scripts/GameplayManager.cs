using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    [SerializeField] Transform[] backgrounds;
    [SerializeField] float backgroundSpeed = 1;
    [SerializeField] float lastPoint;
    [SerializeField] float initialPoint;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        foreach (var item in backgrounds)
        {
            item.position -= new Vector3(1, 0, 0) * backgroundSpeed * Time.deltaTime;
            if (item.position.x < lastPoint)
            {
                item.localPosition = new Vector3(initialPoint, item.localPosition.y, item.localPosition.z);
            }
        }
    }
}
