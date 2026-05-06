using System.Drawing;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i<50; i++)
        {
            SpawnFood();
        }
    }
    public void SpawnFood()
    {

        Vector3 spawnPosition;
        int attempts = 0;
        bool found = false;
        if(currentFood >= maxFood) return;
    
        do
        {
            spawnPosition = new Vector3(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y), 1);
            if (Physics2D.OverlapCircle(spawnPosition, minDistance))
            {
                found = true;   
                break;
            }
            attempts++;
        } while (attempts < 20);
       if (!found) return;
        GameObject _food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        _food.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f,1f,0.9f,1f,0.9f,1f);
        currentFood++;
    }
    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * size, Time.deltaTime * 5f);
    }
  
}

