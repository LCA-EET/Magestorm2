using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using MPTK.NAudio.Midi;
using MidiPlayerTK;

namespace Magus
{
    public class MusicLoader : MonoBehaviour
    {
        private string[] _clipTypes = new string[] { "*.mp3", "*.ogg", "*.wav" };
        private void Awake()
        {
            if (Game.MusicFiles == null)
            {
                Game.MusicFiles = new List<MusicFile>();
                foreach(string fileType in _clipTypes)
                {
                    string[] files = Directory.GetFiles(Application.streamingAssetsPath + "//music//", fileType);
                    foreach (string file in files)
                    {
                        StartCoroutine(FetchAudioClip(file));
                    }
                }
                MidiPlayerGlobal.CurrentMidiSet = new MidiSet();

                string[] midi = Directory.GetFiles(Application.streamingAssetsPath + "//music//", "*.midi");
                foreach (string file in midi)
                {
                    StartCoroutine(FetchMIDI(file));
                }
                Game.MusicFiles.Sort();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static IEnumerator FetchAudioClip(string path)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                    Game.MusicFiles.Add(new MusicFile(path, myClip));
                    Debug.Log("Added " + myClip.ToString());
                }
            }
        }

        public static IEnumerator FetchMIDI(string path)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    byte[] data = www.downloadHandler.data;
                    MidiFile midi = new MidiFile(data, false);
                    MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Add(path);
                    Game.MusicFiles.Add(new MusicFile(path, midi));
                }
            }
        }
    }
}

