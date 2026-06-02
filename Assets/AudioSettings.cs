using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider volumeSlider;

    void Start()
    {
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        musicSource.volume = volume;
        volumeSlider.value = volume;

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    public void ChangeVolume(float volume)
    {
        musicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}