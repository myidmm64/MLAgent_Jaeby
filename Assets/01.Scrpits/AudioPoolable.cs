using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using UnityEngine.Audio;

public class AudioPoolable : PoolableObject
{
    private AudioSource _audioSource = null;

    public override void PopInit()
    {
    }

    public override void PushInit()
    {
    }

    public override void StartInit()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float pitch = 1f, float volume = 1f, AudioMixerGroup mixerGroup = null)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.outputAudioMixerGroup = mixerGroup;
        _audioSource.Play();

        if (pitch < 0)
        {
            _audioSource.time = clip.length * 0.99f;
        }
        else
        {
            _audioSource.time = 0;
        }

        StartCoroutine(WaitForPush((_audioSource.clip.length / Mathf.Abs(_audioSource.pitch)) * 1.05f));
    }

    IEnumerator WaitForPush(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        PoolManager.Instance.Push(this);
    }
}
