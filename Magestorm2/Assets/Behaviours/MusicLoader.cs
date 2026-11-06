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
        void Start()
        {
            if (Game.MusicFiles == null)
            {
                Game.MusicFiles = new List<MusicFile>();
                string[] files = Directory.GetFiles(Application.streamingAssetsPath + "//music//mp3//", "*.mp3");
                foreach (string file in files)
                {
                    StartCoroutine(FetchAudioClip(file));
                }
                files = Directory.GetFiles(Application.streamingAssetsPath + "//music//midi//", "*.bytes");
                foreach (string file in files)
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
                    Game.MusicFiles.Add(new MusicFile(path, midi));
                }
            }
        }
    }
}

