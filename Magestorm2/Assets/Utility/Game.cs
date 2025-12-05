using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using UnityEngine.InputSystem.Controls;
using System.Text;
public static class Game
{
    public const float TickInterval = 0.01f; // 10ms
    public static bool Running;
    private static long _serverTime;
    private static bool _init = false;

    public static bool MenuMode = false;
    public static bool ChatMode = false;
    public static bool ControlMode = false;
    public static bool MouseMode = false;
    
    public static bool GameMode
    {
        get
        {
            return !ChatMode && !MenuMode && !ControlMode;
        }
    }

    public static void Quit()
    {
        if (!EditorApplication.isPlaying)
        {
            Running = false;
            UDPBuilder.StopAllListeners();
            Application.Quit();
        }
    }
    public static void SendPregameBytes(byte[] unencrypted)
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(unencrypted);
    }
    public static void SendInGameBytes(byte[] unencrypted)
    {
        ComponentRegister.InGamePacketProcessor.SendBytes(unencrypted);
    }
    public static void MessageBoxReference(int referenceID)
    {
        MessageBox(Language.GetBaseString(referenceID));
    }
    public static void MessageBox(string message)
    {
        ComponentRegister.UIPrefabManager.InstantiateMessageBox(message);
    }

    public static void YesNo(string message, ValidatableForm instantiator)
    {
        ComponentRegister.UIPrefabManager.InstantiateYesNoBox(message, instantiator);
    }
    public static void Init()
    {
        if (!_init)
        {
            Colors.Init();
            Language.Init();
            LayerManager.Init();
            LevelData.Init();
            Teams.Init();
            MatchOption.Init();
            ActiveMatches.Init();
            ProfanityChecker.Init();
            _init = true;
        }
    }
    private static int ComputeChecksum(byte[] data)
    {
        int toReturn = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > 127)
            {
                toReturn += data[i] - 256;
                // In Java, the first bit of the byte is the sign byte. Java bytes range from -128 to 127. The bytes in C# in contrast are unsigned, so the first bit adds 2^7 (128) to the total. So, I have to subtract 128 * 2 for the checksum on the client (C#) to match that of the server (Java).
            }
            else
            {
                toReturn += data[i];
            }
        }
        return toReturn;
    }
    public static void SetServerTime(long serverTime)
    {
        _serverTime = serverTime;
        Debug.Log("Server time: " + _serverTime);
    }
    public static int FetchServerInfo()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true
                };
                Task<string> t = client.GetStringAsync("https://www.fosiemods.net/ms2.php?func=serverinfo&appid=ms2");
                string returned = t.Result;
                string[] returnedArray = returned.Split("<br>");
                int portNumber = int.Parse(returnedArray[0]);
                string key64 = returnedArray[1];
                Debug.Log("key64: " + key64);
                byte[] key = Convert.FromBase64String(key64);
                //Debug.Log("Key checksum: " + ComputeChecksum(key) + ", Key Length: " + key.Length);
                UDPBuilder.Init("fosiemods.net");
                Cryptography.Init(key);
                return UDPBuilder.CreateClient(portNumber);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        return -1;
    }
}
