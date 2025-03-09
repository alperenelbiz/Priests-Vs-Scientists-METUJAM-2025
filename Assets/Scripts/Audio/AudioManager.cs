using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonBehaviour<AudioManager>
{

    [Header("AudioSourcePrefab")]
    [SerializeField] private AudioSource audioSourcePrefab;

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    [Header("Audio Clip")]
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip walkingSound;
    [SerializeField] AudioClip throwLightSound;
    [SerializeField] AudioClip returnLightSound;
    [SerializeField] AudioClip characterRespawnSound;
    [SerializeField] AudioClip platformRotateSound;
    [SerializeField] AudioClip gateSound;


    private Dictionary<AudioClip, AudioSource> soundToSourceMap = new Dictionary<AudioClip, AudioSource>();


    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource source = GetOrCreateAudioSource();

        if (source == null)
        {
            Debug.LogWarning("Audio source is null or destroyed. Cannot play sound.");
            return;
        }

        if (!source.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Audio source is disabled. Enabling it.");
            source.gameObject.SetActive(true);
        }

        source.PlayOneShot(clip);
    }

    private AudioSource GetOrCreateAudioSource()
    {
        foreach (var source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        if (audioSourcePrefab != null)
        {
            AudioSource newSource = Instantiate(audioSourcePrefab, transform);
            audioSources.Add(newSource);
            return newSource;
        }

        Debug.LogWarning("AudioSource prefab is not assigned.");
        return null;
    }

    public bool IsSoundPlaying(AudioClip clip)
    {
        foreach (var source in audioSources)
        {
            // Check if the source is playing the specific clip
            if (source.isPlaying && source.clip == clip)
            {

                return true; // The clip is currently playing
            }
        }
        return false; // No source is playing the clip
    }


    public void PlayWalkingSound()
    {
        PlaySound(walkingSound);
    }

    public void PlayCharacterRespawnSound()
    {
        PlaySound(characterRespawnSound);
    }

    public void PlayThrowingLightSound()
    {
        PlaySound(throwLightSound);
    }

    public void PlayReturnLightSound()
    {
        PlaySound(returnLightSound);
    }

    public void PlayPlatformRotateSound()
    {
        PlaySound(platformRotateSound);
    }
    public void PlayGateSound()
    {
        PlaySound(gateSound);
    }
    public void PauseAllSounds()
    {
        foreach (var source in audioSources)
        {
            source.Pause();
        }
        musicSource.Pause();
    }
    public void ResumeAllSounds()
    {
        foreach (var source in audioSources)
        {
            if (source.clip != null)
                source.UnPause();
        }
        musicSource.UnPause();
    }

}