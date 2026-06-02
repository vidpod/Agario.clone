using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public GameObject CustomizePanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }


    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void openCustomization()
    {
        mainMenuPanel.SetActive(false);
        CustomizePanel.SetActive(true);
    }

        public void closeCustomization()
    {
        mainMenuPanel.SetActive(true);
        CustomizePanel.SetActive(false);
    }


    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}