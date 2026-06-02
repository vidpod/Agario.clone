using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomization : MonoBehaviour
{
    public TMP_InputField nameInput;

    // UI preview
    public Image avatarPreview;

    // 👇 PLAYER v igri
    public SpriteRenderer playerAvatarRenderer;

    private string imagePath;

    private void Start()
    {
        nameInput.text = PlayerPrefs.GetString("PlayerName", "Player");

        imagePath = PlayerPrefs.GetString("AvatarPath", "");

        if (File.Exists(imagePath))
        {
            LoadAvatar(imagePath);
        }
    }

    public void SaveCustomization()
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);
        PlayerPrefs.SetString("AvatarPath", imagePath);
        PlayerPrefs.Save();
    }

    public void SetImagePath(string path)
    {
        imagePath = path;
        LoadAvatar(path);
    }

public SpriteMask spriteMask; // ✅ DODAJ

   private void LoadAvatar(string path)
{
    byte[] imageBytes = File.ReadAllBytes(path);
    Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);

    if (tex.LoadImage(imageBytes))
    {
        tex.Apply();

        // ✅ AVTOMATSKI PPU
        float maxDimension = Mathf.Max(tex.width, tex.height);
        float desiredSize = 0.9f; // 90% kroga
        float ppu = maxDimension / desiredSize;

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            ppu
        );

        if (avatarPreview != null)
            avatarPreview.sprite = sprite;

        if (playerAvatarRenderer != null)
            playerAvatarRenderer.sprite = sprite;
    }
}
}