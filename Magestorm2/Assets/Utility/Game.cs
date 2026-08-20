using UnityEngine;
using System;
using System.Net;
public static class Game
{
    public const bool SymmetricEncryption = false;
    public static float TickInterval; 
    public static float MovementPolling;
    public static bool Running;
    private static long _serverTime;
    private static bool _init = false;
    public static bool ForceLogin = true;
    public static bool Disconnected = false;
    public static bool MenuMode = false;
    public static bool ChatMode = false;
    public static bool ControlMode = false;
    public static bool MouseMode = false;
    public static UDPGameClient UDP;
    public static UIAudioPlayer UIAudio;
    public static InGameClips Clips;
    public static int GameServerPort;
    public static IPAddress GameServerAddress;
    public static bool LoggedIn = false;
    //public static PMDByte PlayerPMDByte;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        #if !(UNITY_EDITOR)
                    Running = false;
                    UDP.StopListening();
                    Application.Quit();
        #endif
    }
    public static bool GameInputSet(InputControl toCheck)
    {
        return ComponentRegister.InputController.IsSet(toCheck, GameMode);
    }
    public static bool InputSet(InputControl toCheck, bool gameMode)
    {
        return ComponentRegister.InputController.IsSet(toCheck, gameMode);
    }
    public static void SendPregameBytes(byte[] unencrypted)
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(unencrypted);
    }
    public static void SendJoinMatchPacket()
    {
        ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.MatchJoinedPacket(InGame_Send.JoinedMatch));
    }
    public static void SendInGameBytes(byte[] unencrypted)
    {
        if (MatchParams.JoinedMatch)
        {
            ComponentRegister.InGamePacketProcessor.SendBytes(unencrypted);
        }
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
            Language.Init();
            Application.runInBackground = true;
            IconLibrary.Init();
            Colors.Init();
            LayerManager.Init();
            LevelData.Init();
            Teams.Init();
            MatchOption.Init();
            ActiveMatches.Init();
            ProfanityChecker.Init();
            SpellIcons.Init();
            SharedFunctions.Initialize();
            Match.Init();
            _init = true;
        }
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
                Debug.Log("Game Server Port: " + GameServerPort);
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
