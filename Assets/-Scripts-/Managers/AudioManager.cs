using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("AudioManager");
                    _instance = singletonObject.AddComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AudioSource audioSourcePrefab;

    private static readonly float minAudioVolume = 0.0001f;
    private static readonly float maxAudioVolume = 1f;
    public float MinAudioVolume => minAudioVolume;
    public float MaxAudioVolume => maxAudioVolume;

    private Stack<AudioSource> audioSourcesPool = new();

    public static readonly string MasterVolume = "MasterVolume";
    public static readonly string MusicVolume = "MusicVolume";
    public static readonly string SoundFXVolume = "SoundFXVolume";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat(MasterVolume, Mathf.Log10(level) * 20);
        Debug.Log(Mathf.Log10(level) * 20);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat(MusicVolume, Mathf.Log10(level) * 20);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat(SoundFXVolume, Mathf.Log10(level) * 20);
    }

    public void PlayAudioClip(AudioClip clip, Transform spawnPoint)
    {
        PlayAudioClip(clip, spawnPoint, 1f);
    }

    public void PlayAudioClip(AudioClip clip, Transform spawnPoint, float volume)
    {
        AudioSource audioSource;

        if (audioSourcesPool.Count <= 0)
        {
            audioSource = Instantiate(audioSourcePrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            audioSource = audioSourcesPool.Pop();
            audioSource.transform.position = spawnPoint.position;
            audioSource.gameObject.SetActive(true);
            audioSource.transform.SetParent(null);
        }

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        StartCoroutine(ReturnAudioSourceToPool(audioSource, clipLength));
    }

    public void PlayRandomAudioClip(List<AudioClip> clips, Transform spawnPoint)
    {
        PlayRandomAudioClip(clips, spawnPoint, 1f);
    }

    public void PlayRandomAudioClip(List<AudioClip> clips, Transform spawnPoint, float volume)
    {
        int rand = Random.Range(0, clips.Count);
        PlayAudioClip(clips[rand], spawnPoint, volume);
    }

    private IEnumerator ReturnAudioSourceToPool(AudioSource audioSource, float clipLenght)
    {
        yield return new WaitForSeconds(clipLenght);
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.gameObject.SetActive(false);
        audioSource.transform.SetParent(transform);
        audioSourcesPool.Push(audioSource);
    }

}
