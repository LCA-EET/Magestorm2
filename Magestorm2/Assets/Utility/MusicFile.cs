using MPTK.NAudio.Midi;
using System;
using UnityEngine;
public class MusicFile : IComparable<MusicFile>
{
    private readonly MidiFile _midi;
    private readonly AudioClip _clip;
    private string _path;
    public MusicFile(string path, AudioClip audioClip)
    {
        _clip = audioClip;
        _path = path.ToLower();
    }
    public MusicFile(string path, MidiFile midiFile)
    {
        _midi = midiFile;
        _path = path.ToLower();
    }
    public bool IsMidi
    {
        get
        {
            return _midi != null;
        }
    }
    public MidiFile MidiFile
    {
        get
        {
            return _midi;
        }
    }

    public AudioClip AudioClip
    {
        get
        {
            return _clip;
        }
    }
    public int CompareTo(MusicFile other)
    {
        return Path.CompareTo(other.Path);
    }

    public string Path
    {
        get { return _path; }
    }
    public void Play()
    {
        
    }
}
