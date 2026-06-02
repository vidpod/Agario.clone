using System.IO;
using UnityEngine;

public class PlayerAvatarLoader : MonoBehaviour
{
    public SpriteRenderer avatarRenderer;
    public Transform avatarTransform;

    void Start()
    {
        string path = PlayerPrefs.GetString("AvatarPath", "");

        if (string.IsNullOrEmpty(path)) return;
        if (!File.Exists(path)) return;

        byte[] bytes = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        if (!tex.LoadImage(bytes))
        {
            Debug.LogError("Slika se ni naložila!");
            return;
        }

        tex.Apply();

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            200f
        );

        avatarRenderer.sprite = sprite;
        FitInsideCircle(tex);
    }

    void FitInsideCircle(Texture2D tex)
    {
        float aspect = (float)tex.width / tex.height;
        float size = 0.30f;

        if (aspect > 1f)
            avatarTransform.localScale = new Vector3(size, size / aspect, 1f);
        else
            avatarTransform.localScale = new Vector3(size * aspect, size, 1f);
    }
}