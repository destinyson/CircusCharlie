using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    private AudioSource source;
    private int clipOrder;
    private bool loop;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            source = GetComponent<AudioSource>();
            clipOrder = 0;
            loop = false;
        }

        else if (this != _instance)
            Destroy(gameObject);
    }

    public void playAudioClip(AudioClip clip, bool loop)
    {
        clipOrder = 0;
        this.loop = loop;

        if (isLoop())
            source.Stop();

        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    public bool playAudioClipFromList(AudioClip[] clipList)
    {
        if (clipOrder == clipList.Length)
            return false;

        if (isLoop())
            source.Stop();
        loop = false;

        if (!isPlaying())
        {
            source.clip = clipList[clipOrder];
            source.loop = loop;
            source.Play();
            ++clipOrder;
        }
        return true;
    }

    public bool isLoop()
    {
        return loop;
    }

    public bool isPlaying()
    {
        return source.isPlaying;
    }

    public void pause()
    {
        source.Pause();
    }

    public void play()
    {
        source.Play();
    }
}