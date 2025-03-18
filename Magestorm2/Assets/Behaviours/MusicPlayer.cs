using System.Runtime.CompilerServices;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource _musicSource;
    private bool _playMusic = true;
    private int _clipIndex;
    public AudioClip[] MusicClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _musicSource = GetComponent<AudioSource>();
        _clipIndex = MusicClips.Length;
        PlayNextClip();
    }
    // Update is called once per frame
    void Update()
    {
        if (InputControls.ToggleMusic)
        {
            _playMusic = !_playMusic;
            if (!_playMusic)
            {
                _musicSource.Stop();
            }
            if (_playMusic)
            {
                PlayNextClip();
            }
        }
        else
        {
            if (InputControls.NextTrack)
            {
                PlayNextClip();
            }
            else if (InputControls.PreviousTrack)
            {
                PlayPreviousClip();
            }
        }
    }
    private void PlayNextClip()
    {
        _musicSource.Stop();
        if (_clipIndex == MusicClips.Length)
        {
            _clipIndex = 0;
        }
        else
        {
            _clipIndex++;
        }
        PlayTrack();
    }
    private void PlayPreviousClip()
    {
        _musicSource.Stop();
        if (_clipIndex == 0)
        {
            _clipIndex = MusicClips.Length - 1;
        }
        else
        {
            _clipIndex--;
        }
        PlayTrack();
    }
    private void PlayTrack()
    {
        _musicSource.clip = MusicClips[_clipIndex];
        _musicSource.Play();
    }
}
