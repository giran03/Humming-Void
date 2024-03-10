using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Sound[] musicSounds, sfxSounds;
    [SerializeField] AudioSource musicSource, sfxSource;

    public static AudioManager Instance;
    AudioSource currentLoopingAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start() => PlayMusic("Ambience");

    public void PlayMusic(string clipName)
    {
        Sound sound = Array.Find(musicSounds, x => x.clipName == clipName);
        if (sound != null)
        {
            musicSource.clip = sound.audioClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(string clipName, Vector3 position, bool loop = false)
    {
        Sound sound = Array.Find(sfxSounds, x => x.clipName == clipName);
        if (sound != null)
        {
            sfxSource.loop = loop;

            if (loop)
            {
                if (currentLoopingAudioSource != null)
                {
                    currentLoopingAudioSource.Stop();
                }

                sfxSource.clip = sound.audioClip;
                sfxSource.Play();
                currentLoopingAudioSource = sfxSource;
            }
            else
            {
                AudioSource.PlayClipAtPoint(sound.audioClip, position);
                StartCoroutine(Cooldown(sound.audioClip));
            }
        }
    }

    public void StopSFX()
    {
        if (currentLoopingAudioSource != null)
        {
            currentLoopingAudioSource.Stop();
            currentLoopingAudioSource = null;
        }
    }

    IEnumerator Cooldown(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
    }

}
