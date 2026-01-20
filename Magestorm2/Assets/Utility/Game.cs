using UnityEngine;
using System;
using System.Net;
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
    public static UDPGameClient UDP;
    public static int GameServerPort;
    public static IPAddress GameServerAddress;
    public static bool LoggedIn = false;
    public static bool GameMode
    {
        get
        {
            return !ChatMode && !MenuMode && !ControlMode;
        }
    }
    public static Avatar PCAvatar
    {
        get { return ComponentRegister.PlayerAvatar; }
    }
    public static void Quit()
    {
        #if !(UNITY_EDITOR)
            Running = false;
            UDPBuilder.StopAllListeners();
            Application.Quit();
        #endif
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
            IconLibrary.Init();
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
        try
        {
            string contents;
            if(SharedFunctions.GetPHPString("serverinfo", out contents))
            {
                string[] returnedArray = contents.Split("<br>");
                GameServerPort = int.Parse(returnedArray[0]);
                string key64 = returnedArray[1];
                byte[] key = Convert.FromBase64String(key64);
                AssignGameServerAddress("fosiemods.net");
                Cryptography.Init(key);
                return GameServerPort;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return -1;
    }
    private static void AssignGameServerAddress(string hostname)
    {
        GameServerAddress = Dns.GetHostAddresses(hostname)[0];
        if (GameServerAddress.ToString().StartsWith("192"))
        {
            GameServerAddress = Dns.GetHostAddresses("apps.home.lan")[0];
        }
        Debug.Log("Server IP: " + GameServerAddress.ToString());
    }
}
