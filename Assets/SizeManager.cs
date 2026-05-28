using UnityEngine;

public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f,growthPerFood = 0.5f;

    public int coins = 0;
    public float maxSizeReached = 1f;

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            currentScale += 0.1f/currentScale;
            coins++;
            GameManager.instance.currentFood--;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Bots"))
        {
            SizeManager otherPlayer = other.GetComponent<SizeManager>();

            if (otherPlayer != null && currentScale > otherPlayer.currentScale * 1.1f)
            {
                int reward = Mathf.RoundToInt(otherPlayer.currentScale * 10f);
                coins += reward;

                currentScale += otherPlayer.currentScale * 0.5f;

                Destroy(other.gameObject);
            }
        }
    }
    void Update()
    {
        if (currentScale > maxSizeReached) 
        { 
            maxSizeReached = currentScale; 
        }
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale,1), Time.deltaTime * scaleSpeed);
    }
    private void Start()
    {
        PlayerData data = Money.Load();

        coins = data.coins;
        currentScale = Mathf.Max(currentScale, data.maxSize);
    }
    private void OnApplicationQuit()
    {
        Money.Save(coins, maxSizeReached);
    }
}
