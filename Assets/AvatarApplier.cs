using System.IO;
using UnityEngine;

public class AvatarApplier : MonoBehaviour
{
    public SpriteRenderer targetRenderer;

    void Start()
    {
        string path = PlayerPrefs.GetString("AvatarPath", "");

        if (string.IsNullOrEmpty(path)) return;
        if (!File.Exists(path)) return;

        byte[] bytes = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );

        targetRenderer.sprite = sprite;
    }
}