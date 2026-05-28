using UnityEngine;
using System.IO;


[System.Serializable]
public class PlayerData
{
    public int coins;
    public float maxSize;
}

public class Money : MonoBehaviour
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void Save(int coins, float maxSize)
    {
        PlayerData data = new PlayerData();
        data.coins = coins;
        data.maxSize = maxSize;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static PlayerData Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }

        return new PlayerData();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
