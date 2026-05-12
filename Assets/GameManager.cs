using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject botPrefab;
    int botCount = 10;
    public GameObject foodPrefab;
    public Vector2 xRange, yRange;
    public float minDistance = 1.5f, size = 1f, maxSize = 3f, growthPerFood  = 0.1f;
    public int currentFood = 0, maxFood = 100;

    public void Grow()
    {
        size += growthPerFood;
        size = Mathf.Clamp(size, 1f, maxSize);
    }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        for (int i = 0; i<50; i++)
        {
            SpawnFood();
        }
        for (int i = 0; i < botCount; i++)
        {
            spawnBot();
        }
    }
    public void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            0
        ); GameObject _food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        _food.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f,1f,0.9f,1f,0.9f,1f);
        currentFood++;
    }

    public void spawnBot()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            0
        );


       
        GameObject _bot = Instantiate(botPrefab, spawnPosition, Quaternion.identity);
        SpriteRenderer sr = _bot.GetComponent<SpriteRenderer>();
       
        Color botColor = Random.ColorHSV(0f, 1f, 0.9f, 1f, 0.9f, 1f);
        sr.material.SetColor("_MainColor", botColor);
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * size, Time.deltaTime * 5f);
    }
  
}

