using UnityEngine;

public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f, growthPerFood = 0.5f;
    public float combatGrowthFactor = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
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

                Debug.Log($"[USPEH] {gameObject.name} je pojedel {collision.gameObject.name}. Nova ciljna velikost: {currentScale}");

                Destroy(collision.gameObject);
            }
        }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale,1), Time.deltaTime * scaleSpeed);
    }
}
