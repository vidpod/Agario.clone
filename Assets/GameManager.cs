using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject foodPrefab;
    public Vector2 xRange, yRange;


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
        Vector3 spawnPosition = new Vector3(Random.Range(xRange.x, yRange.y), Random.Range(xRange.x, yRange.y), 1);
        GameObject _food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        _food.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f,1f,0.9f,1f,0.9f,1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
