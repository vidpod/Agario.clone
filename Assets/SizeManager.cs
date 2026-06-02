using UnityEngine;
using UnityEngine.SceneManagement;
public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f, growthPerFood = 0.5f;
    public float combatGrowthFactor = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {

            SFXManager.instance.PlayFoodEat();

            currentScale += 0.1f/currentScale;
            GameManager.instance.currentFood--;
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SizeManager otherCharacter = collision.gameObject.GetComponent<SizeManager>();

        if (otherCharacter != null)
        {
            
            if (this.currentScale > otherCharacter.currentScale)
{
    currentScale += otherCharacter.currentScale * combatGrowthFactor;

    if (collision.gameObject.CompareTag("Player"))
    {
        SFXManager.instance.PlayPlayerDeath();
        Invoke(nameof(LoadGameOver), 1f);
    }
    else
    {
        SFXManager.instance.PlayBotEat();
    }

    Destroy(collision.gameObject);
}
        }
    }

    private void LoadGameOver()
    {
    SceneManager.LoadScene("GameOver");
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale,1), Time.deltaTime * scaleSpeed);
    }
}
