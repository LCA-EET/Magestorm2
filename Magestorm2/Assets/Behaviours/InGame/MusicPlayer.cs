using MidiPlayerTK;
using UnityEngine;

namespace Magus
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _musicSource;
        private bool _playMusic = true;
        private int _clipIndex;
        private MidiFilePlayer _midiPlayer;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            Game.MusicPlayer = this;
            DontDestroyOnLoad(gameObject);
            _musicSource = GetComponent<AudioSource>();
            _midiPlayer = GetComponentInChildren<MidiFilePlayer>();
        }
        void Start()
        {
            //PlayNextClip();
        }
        public void PlayFirstClip()
        {
            if(Game.MusicFiles.Count >= 1)
            {
                _clipIndex = 0;
                PlayTrack();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (InputControls.ToggleMusic)
            {
                _playMusic = !_playMusic;
                if (!_playMusic)
                {
                    StopTrack();
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
            StopTrack();
            if (_clipIndex == Game.MusicFiles.Count)
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
            StopTrack();
            if (_clipIndex == 0)
            {
                _clipIndex = Game.MusicFiles.Count - 1;
            }
            else
            {
                _clipIndex--;
            }
            PlayTrack();
        }
        private void PlayTrack()
        {
            MusicFile musicFile = Game.MusicFiles[_clipIndex];
            if (musicFile.IsMidi)
            {
                _midiPlayer.PlayMidi(musicFile.MidiFile.GetBytes(), musicFile.Path);
            }
            else
            {
                _musicSource.clip = musicFile.AudioClip;
                _musicSource.Play();
            }
            Debug.Log("Now playing: " + musicFile.Path);
        }

        private void StopTrack()
        {
            _musicSource.Stop();
            _midiPlayer.MPTK_Stop();
        }
    }

}