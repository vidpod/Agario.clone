using UnityEngine;

public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentScale *= 1.85f;
        GameManager.instance.SpawnFood();
        Destroy(collision.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale,1), Time.deltaTime * scaleSpeed);
    }
}
