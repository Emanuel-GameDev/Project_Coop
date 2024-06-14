using TMPro;
using UnityEngine;

public class MenuAudioSettings : MonoBehaviour
{
    [SerializeField]
    private int maxVolume = 10;

    [SerializeField]
    private TextMeshProUGUI masterVolumeText;
    [SerializeField]
    private TextMeshProUGUI masterMusicText;
    [SerializeField]
    private TextMeshProUGUI masterSoundFXText;

    private int actualMasterVolume;
    private int actualMusicVolume;
    private int actualSoundFXVolume;

    private void Awake()
    {
        actualMasterVolume = maxVolume;
        actualMusicVolume = maxVolume;
        actualSoundFXVolume = maxVolume;
        masterVolumeText.text = actualMasterVolume.ToString();
        masterMusicText.text = actualMusicVolume.ToString();
        masterSoundFXText.text = actualSoundFXVolume.ToString();
    }

    public void IncreaseMasterVolume()
    {
        if (actualMasterVolume < maxVolume)
            actualMasterVolume++;

        ModifyMasterVolume();
    }

    public void DecreaseMasterVolume()
    {
        if (actualMasterVolume > 0)
            actualMasterVolume--;

        ModifyMasterVolume();
    }

    public void IncreaseMusicVolume()
    {
        if (actualMusicVolume < maxVolume)
            actualMusicVolume++;

        ModifyMusicVolume();
    }

    public void DecreaseMusicVolume()
    {
        if (actualMusicVolume > 0)
            actualMusicVolume--;

        ModifyMusicVolume();
    }

    public void IncreaseSoundFXVolume()
    {
        if (actualSoundFXVolume < maxVolume)
            actualSoundFXVolume++;

        ModifySoundFXVolume();
    }


    public void DecreaseSoundFXVolume()
    {
        if (actualSoundFXVolume > 0)
            actualSoundFXVolume--;

        ModifySoundFXVolume();
    }

    private void ModifyMasterVolume()
    {
        masterVolumeText.text = actualMasterVolume.ToString();
        float volume = (float)actualMasterVolume / (float)maxVolume;
        volume = volume <= 0 ? AudioManager.Instance.MinAudioVolume : volume;
        AudioManager.Instance.SetMasterVolume(volume);

    }

    private void ModifyMusicVolume()
    {
        masterMusicText.text = actualMusicVolume.ToString();
        float volume = (float)actualMusicVolume / (float)maxVolume;
        volume = volume <= 0 ? AudioManager.Instance.MinAudioVolume : volume;
        AudioManager.Instance.SetMusicVolume(volume);
    }

    private void ModifySoundFXVolume()
    {
        masterSoundFXText.text = actualSoundFXVolume.ToString();
        float volume = (float)actualSoundFXVolume / (float)maxVolume;
        volume = volume <= 0 ? AudioManager.Instance.MinAudioVolume : volume;
        AudioManager.Instance.SetSoundFXVolume(volume);
    }
}
