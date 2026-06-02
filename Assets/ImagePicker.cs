using SFB;
using UnityEngine;

public class ImagePicker : MonoBehaviour
{
    public PlayerCustomization customization;

    public void PickImage()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Choose Avatar",
            "",
            new[] { new SFB.ExtensionFilter("Images", "png", "jpg", "jpeg") },
            false
        );

        if (paths.Length > 0)
        {
            customization.SetImagePath(paths[0]);
        }
    }
}