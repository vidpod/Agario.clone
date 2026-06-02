using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource sfxSource;

    public AudioClip foodEatSound;
    public AudioClip botEatSound;
    public AudioClip playerDeathSound;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayFoodEat()
    {
        sfxSource.PlayOneShot(foodEatSound);
    }

    public void PlayBotEat()
    {
        sfxSource.PlayOneShot(botEatSound);
    }

    public void PlayPlayerDeath()
    {
        sfxSource.PlayOneShot(playerDeathSound);
    }
}