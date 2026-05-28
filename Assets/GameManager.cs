using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject botPrefab;
    int botCount = 10;
    public GameObject foodPrefab;
    public Vector2 xRange, yRange;


    private float foodTimer;
    public int currentFood = 0, maxFood = 300;

    public float minSpawnTime = 0.2f;
    public float maxSpawnTime = 2f;

    private float nextSpawnTime,cellSize = 10f;
    private float timer;

    
    private void Awake()
    {
        instance = this;
      
    }
    void Start()
    {
     
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        for (int i = 0; i < botCount; i++)
        {
            spawnBot();
        }
    }
    public void SpawnFood()
    {
       

        int xCell = Random.Range(0, Mathf.RoundToInt((xRange.y - xRange.x) / cellSize));
        int yCell = Random.Range(0, Mathf.RoundToInt((yRange.y - yRange.x) / cellSize));

        Vector3 spawnPosition = new Vector3(
            xRange.x + xCell * cellSize + Random.Range(0, cellSize),
            yRange.x + yCell * cellSize + Random.Range(0, cellSize),
            0
        );
      
        GameObject _food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);

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
        foodTimer += Time.deltaTime;

        if (foodTimer >= nextSpawnTime)
        {
            foodTimer = 0f;
            SpawnFood();
            nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
  
}

