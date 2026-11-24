using UnityEngine;
using System.Net;
using System.Collections.Generic;

namespace Magus
{
    public static class Game
    {
        public const string ClientVersion = "0.1";
        public static string DownloadURL = "";
        public static string RequiredVersion;
        public const bool SymmetricEncryption = false;
        public static float TickInterval;
        public static string DiscordURL;
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
        public static Dictionary<string, SpellManager> SpellManagerTable;
        public static Dictionary<string, DisciplineManager> DisciplineManagerTable;
        public static Dictionary<string, CharacterClassManager> ClassManagerTable;
        public static ConsumableDisplay ConsumableDisplay;
        public static SpellManager ActiveSpellManager;
        public static DisciplineManager ActiveDisciplineManager;
        public static CharacterClassManager ActiveCharacterClassManager;
        public static List<MusicFile> MusicFiles;
        public static MusicPlayer MusicPlayer;
        public static bool LoggedIn = false;
        public static byte[] PregameKey;
        public static byte Cipher;
        public static bool MusicPlayerCreated;
        
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
            Application.Quit();
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
                TextFieldRegister.Init();
                GameSettings.InitializeSettings();
                Language.Init();
                SpellManagerTable = new Dictionary<string, SpellManager>();
                DisciplineManagerTable = new Dictionary<string, DisciplineManager>();
                ClassManagerTable = new Dictionary<string, CharacterClassManager>();
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
    }
}
