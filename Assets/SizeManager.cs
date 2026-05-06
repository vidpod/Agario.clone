using UnityEngine;

public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        currentScale *= 1.05f;
        GameManager.instance.SpawnFood();
        Destroy(other.gameObject);
    }
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale,1), Time.deltaTime * scaleSpeed);
    }
}
