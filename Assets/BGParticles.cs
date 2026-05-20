using UnityEngine;
using UnityEngine.UI;

public class UIParticleBackground : MonoBehaviour
{
    public RectTransform canvasRect;
    public GameObject dotPrefab;

    public int count = 100;
    public float speed = 20f;

    private RectTransform[] dots;

    void Start()
    {
        if (canvasRect == null || dotPrefab == null)
        {
            Debug.LogError("CanvasRect ali DotPrefab ni nastavljen!");
            return;
        }

        dots = new RectTransform[count];

        float w = canvasRect.rect.width / 2f;
        float h = canvasRect.rect.height / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject dot = Instantiate(dotPrefab, canvasRect);
            RectTransform rt = dot.GetComponent<RectTransform>();

            // position
            rt.anchoredPosition = new Vector2(
                Random.Range(-w, w),
                Random.Range(-h, h)
            );

            // scale
            float s = Random.Range(0.4f, 1.4f);
            rt.localScale = new Vector3(s, s, 1f);

            // COLOR FIX (IMPORTANT)
            Image img = dot.GetComponentInChildren<Image>();

            if (img != null)
            {
                // reset tint (VERY IMPORTANT for UI sprites)
                img.color = Color.white;

                // real visible variation (NOT grey-only)
                Color col = new Color(
                    Random.Range(0.6f, 1f),
                    Random.Range(0.6f, 1f),
                    Random.Range(0.6f, 1f),
                    Random.Range(0.25f, 0.75f)
                );

                img.color = col;
            }

            dots[i] = rt;
        }
    }

    void Update()
    {
        if (dots == null) return;

        float w = canvasRect.rect.width / 2f;
        float h = canvasRect.rect.height / 2f;

        for (int i = 0; i < dots.Length; i++)
        {
            RectTransform d = dots[i];

            Vector2 pos = d.anchoredPosition;

            // smooth drift
            pos += new Vector2(
                Mathf.Sin(Time.time * 0.5f + i),
                Mathf.Cos(Time.time * 0.5f + i)
            ) * speed * Time.deltaTime;

            // wrap
            if (pos.x > w) pos.x = -w;
            if (pos.x < -w) pos.x = w;
            if (pos.y > h) pos.y = -h;
            if (pos.y < -h) pos.y = h;

            d.anchoredPosition = pos;
        }
    }
}