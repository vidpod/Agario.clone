using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroll : MonoBehaviour
{
    public float speedX = 0.02f;
    public float speedY = 0.02f;

    private RawImage image;

    void Start()
    {
        image = GetComponent<RawImage>();
    }

    void Update()
    {
        image.uvRect = new Rect(
            image.uvRect.position + new Vector2(speedX, speedY) * Time.deltaTime,
            image.uvRect.size
        );
    }
}