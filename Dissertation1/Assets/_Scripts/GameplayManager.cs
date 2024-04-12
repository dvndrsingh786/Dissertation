using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    [Header("Backgrounds")]
    [SerializeField] Transform[] backgrounds;
    public float environmentSpeed = 1;
    [SerializeField] float lastPoint;
    [SerializeField] float initialPoint;
    [Header("Fire & Other Objects")]
    [SerializeField] GameObject firePrefab;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] Vector2[] objPositions;
    [SerializeField] List<int> usedPositions = new List<int>();
    [Header("Health And Score UI")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] int score;
    [SerializeField] TextMeshProUGUI scoreTxt;
    [Header("Score Values")]
    public static int fireScore = 10;
    public static int obstacleScore = 30;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitialSetup();
    }

    void InitialSetup()
    {
        AddFire(backgrounds[backgrounds.Length - 1]);
        AddFire(backgrounds[backgrounds.Length - 2]);
    }

    public void GameOver()
    {
        environmentSpeed = 0;
    }

    private void Update()
    {
        foreach (var item in backgrounds)
        {
            item.position -= new Vector3(1, 0, 0) * environmentSpeed * Time.deltaTime;
            if (item.position.x < lastPoint)
            {
                item.localPosition = new Vector3(initialPoint, item.localPosition.y, item.localPosition.z);
                usedPositions = new List<int>();
                RemoveObjs(item);
                AddFire(item);
                AddObstacle(item);
            }
        }
    }

    void AddFire(Transform parent)
    {
        int fireCount = Random.Range(1, 3);
        if (fireCount > 0)
        {
            for (int i = 0; i < fireCount; i++)
            {
                GameObject fire = Instantiate(firePrefab, parent);
                int positionIndex = Random.Range(0, objPositions.Length);
                while (usedPositions.Contains(positionIndex))
                {
                    positionIndex = Random.Range(0, objPositions.Length);
                }
                fire.transform.localPosition = objPositions[positionIndex];
                usedPositions.Add(positionIndex);
            }
        }
    }

    void AddObstacle(Transform parent)
    {
        GameObject obstacle = Instantiate(obstaclePrefab, parent);
        int positionIndex = Random.Range(0, objPositions.Length);
        while (usedPositions.Contains(positionIndex))
        {
            positionIndex = Random.Range(0, objPositions.Length);
        }
        obstacle.transform.localPosition = objPositions[positionIndex];
        usedPositions.Add(positionIndex);
    }

    void RemoveObjs(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(0).gameObject);
        }
    }

    public void SetHealthUI(int _health)
    {
        healthBar.fillAmount = _health;
        healthValue.text = _health.ToString();
    }

    public void AddScore(int _scoreToAdd)
    {
        score += _scoreToAdd;
        scoreTxt.text = "Score: " + score;
    }
}
