using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PregamePacketProcessor : UDPProcessor
{
    private PeriodicAction _heartbeat;
    private void Awake()
    {
        ComponentRegister.PregamePacketProcessor = this;
        _heartbeat = new PeriodicAction(30.0f, Heartbeat, null);
        Init(Game.GameServerPort);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.UDP.HasPacketsPending)
        {
            List<byte[]> toProcess = Game.UDP.PacketsReceived();
            foreach (byte[] decryptedPayload in toProcess)
            {
                PreProcess(decryptedPayload);
                switch (_opCode)
                {
                    case Pregame_Receive.CreationFailed:
                        MessageBox(27);
                        break;
                    case Pregame_Receive.AccountCreated:
                        MessageBox(25);
                        break;
                    case Pregame_Receive.AccountAlreadyExists:
                        MessageBox(26);
                        break;
                    case Pregame_Receive.LogInFailed:
                        MessageBox(28);
                        break;
                    case Pregame_Receive.LogInSucceeded:
                        HandleLogInSuccessfulPacket();
                        break;
                    case Pregame_Receive.ProhibitedLanguage:
                        MessageBox(30);
                        break;
                    case Pregame_Receive.AlreadyLoggedIn:
                        MessageBox(31);
                        break;
                    case Pregame_Receive.RemovedFromServer:
                    case Pregame_Receive.InactivityDisconnect:
                        QuitWithMessage(32);
                        break;
                    case Pregame_Receive.CharacterExists:
                        MessageBox(34);
                        break;
                    case Pregame_Receive.CharacterCreated:
                        HandleCharacterCreatedPacket();
                        break;
                    case Pregame_Receive.CharacterDeleted:
                        HandleCharacterDeletedPacket();
                        break;
                    case Pregame_Receive.MatchStillHasPlayers:
                        MessageBox(49);
                        break;
                    case Pregame_Receive.MatchLimitReached:
                        MessageBox(47);
                        break;
                    case Pregame_Receive.MatchAlreadyCreated:
                        MessageBox(46);
                        break;
                    case Pregame_Receive.MatchData:
                        HandleMatchDataPacket();
                        break;
                    case Pregame_Receive.LevelsList:
                        HandleLevelListPacket();
                        break;
                    case Pregame_Receive.BannedForBehavior:
                        QuitWithMessage(71);
                        break;
                    case Pregame_Receive.BannedForCheating:
                        QuitWithMessage(70);
                        break;
                    case Pregame_Receive.MatchDetails:
                        HandleMatchDetailsPacket();
                        break;
                    case Pregame_Receive.NameCheckResult:
                        HandleNameCheckResultPacket();
                        break;
                    case Pregame_Receive.MatchIsFullPacket:
                        HandleMatchIsFullPacket();
                        break;
                    case Pregame_Receive.MatchEntryPacket:
                        HandleMatchEntryPacket();
                        break;
                    case Pregame_Receive.AcknowledgeSubscription:
                        ComponentRegister.UIPrefabManager.InstantiateMatchList();
                        break;
                    case Pregame_Receive.UpdateSkillsAndSlots:
                        HandleSkillsSlotsUpdate();
                        break;
                    case Pregame_Receive.ExpLevelUpdate:
                        HandleExperienceUpdate();                
                        break;
                    case Pregame_Receive.MatchScore:
                        HandleMatchScoreRequest();
                        break;
                }
            }
        }
        if (Game.LoggedIn)
        {
            _heartbeat.ProcessAction(Time.deltaTime);
        }
    }
    private void HandleMatchScoreRequest()
    {
        ComponentRegister.UIPrefabManager.InstantiateMatchScores(_decrypted, true);
    }
    private void HandleExperienceUpdate()
    {
        int characterID = BitConverter.ToInt32(_decrypted, 1);
        byte characterLevel = _decrypted[5];
        int experience = BitConverter.ToInt32(_decrypted, 6);
        PlayerCharacter pc = PlayerAccount.GetCharacter(characterID);
        if(pc != null)
        {
            pc.SetExperience(experience);
            pc.SetLevel(characterLevel);
            PlayerAccount.UpdatesMade = true;
        }
    }
    private void QuitWithMessage(int messageReference)
    {
        ComponentRegister.UIPrefabManager.InstantiateMessageBox_Function(Language.GetBaseString(messageReference), Game.Quit);
    }
    private void Heartbeat()
    {
        Game.SendPregameBytes(Pregame_Packets.HeartbeatPacket());
    }
    private void HandleSkillsSlotsUpdate()
    {
        int characterID = BitConverter.ToInt32(_decrypted, 5);
        int skillsInt = BitConverter.ToInt32(_decrypted, 9);
        PlayerCharacter toUpdate = PlayerAccount.GetCharacter(characterID);
        toUpdate.UpdateSkillsTable(skillsInt);
        toUpdate.UpdateSlottedSpells(_decrypted, 13);
    }
    private void HandleMatchEntryPacket()
    {
        MatchParams.Init(_decrypted);
        Game.UDP.StopListening();
        ComponentRegister.UIPrefabManager.ClearStack();
        Debug.Log("SceneID: " + MatchParams.SceneID);
        SceneManager.LoadScene(MatchParams.SceneID.ToString());
    }
    private void HandleMatchIsFullPacket()
    {
        Game.MessageBoxReference(101);
    }
    private void HandleNameCheckResultPacket()
    {
        byte isUsed = _decrypted[1];
        switch (isUsed)
        {
            case 0:
                ComponentRegister.UIPCEditor.NameCheckPassed();
                break;
            case 1:
                Game.MessageBoxReference(96);
                break;
            case 2:
                Game.MessageBoxReference(97);
                break;
        }
    }
    private void HandleMatchDetailsPacket()
    {
        
        byte matchID = _decrypted[1];
        int index = 2;
        RemotePlayerData[] neutralPlayers = ProcessMatchPlayers(ref index);
        RemotePlayerData[] chaosPlayers = ProcessMatchPlayers(ref index);
        RemotePlayerData[] balancePlayers = ProcessMatchPlayers(ref index);
        RemotePlayerData[] orderPlayers = ProcessMatchPlayers(ref index);
        ListedMatch match = null;
        if(ActiveMatches.GetMatch(matchID, ref match))
        {
            SharedFunctions.Params = new object[] { match, chaosPlayers, balancePlayers, orderPlayers };
            ComponentRegister.UIPrefabManager.InstantiateJoinMatch();
        }      
    }
    private RemotePlayerData[] ProcessMatchPlayers(ref int index)
    {
        byte numPlayers = _decrypted[index];
        Debug.Log("Index: " + index);
        RemotePlayerData[] toReturn = new RemotePlayerData[numPlayers];
        index++;
        int playerIndex = 0;
        Debug.Log("NumPlayers: " + numPlayers);
        
        
        if (playerIndex < numPlayers)
        {
            Debug.Log("Processing player: " + playerIndex);
            byte idInMatch = _decrypted[index];
            index++;
            byte teamID = _decrypted[index];
            index++;
            byte[] appearanceBytes = new byte[5];
            Array.Copy(_decrypted, index, appearanceBytes, 0, appearanceBytes.Length);
            index += 5;
            byte playerLevel = _decrypted[index];
            index++;
            byte playerClass = _decrypted[index];
            index++;
            byte nameLength = _decrypted[index];
            index++;
            string playerName = ByteUtils.BytesToUTF8(_decrypted, index, nameLength);
            Debug.Log(playerName);
            index += nameLength;
            toReturn[playerIndex] = new RemotePlayerData(idInMatch, teamID, playerName, playerLevel, playerClass);
            playerIndex++;
        }
        
        return toReturn;
    }
    private void HandleLevelListPacket()
    {
        byte numLevels = _decrypted[1];
        byte levelIdx = 0;
        int index = 2;
        while(levelIdx < numLevels)
        {
            byte sceneID = _decrypted[index];
            index++;
            byte maxPlayers = _decrypted[index];
            index++;
            byte poolLength = _decrypted[index];
            index++;
            byte[] poolData = new byte[poolLength];
            Array.Copy(_decrypted, index, poolData, 0, poolLength);
            index += poolLength;
            byte nameLength = _decrypted[index];
            index++;
            byte[] nameBytes = new byte[nameLength];
            Array.Copy(_decrypted, index, nameBytes, 0, nameLength);
            index += nameLength;
             levelIdx++;
            LevelData.AddLevel(sceneID, maxPlayers, Encoding.UTF8.GetString(nameBytes), poolData);
        }
    }
    private void HandleMatchDataPacket()
    {
        byte matchCount = _decrypted[1];
        int index = 2;
        ActiveMatches.ClearMatches();
        for(int i = 0; i < matchCount; i++)
        {
            byte matchID = _decrypted[index];
            index++;
            byte sceneID = _decrypted[index];
            index++;
            long expirationTime = BitConverter.ToInt64(_decrypted, index);
            long currentTime = TimeUtil.CurrentTime();
            index += 8;
            int creatorAccountID = BitConverter.ToInt32(_decrypted, index);
            index += 4;
            byte nameLength = _decrypted[index];
            byte[] nameBytes = new byte[nameLength];
            index++;
            byte matchType = _decrypted[index];
            index++;
            byte matchOptions = _decrypted[index];
            index++;
            Array.Copy(_decrypted, index, nameBytes, 0, nameLength);
            string creatorName = Encoding.UTF8.GetString(nameBytes);
            index += nameLength;
            ListedMatch toAdd = new ListedMatch(matchID, sceneID, creatorName, expirationTime, creatorAccountID, matchType, matchOptions);
            ActiveMatches.AddMatch(toAdd);
        }
    }
    private void MessageBox(int stringReference)
    {
        Game.MessageBox(Language.GetBaseString(stringReference));
    }
    private void HandleCharacterDeletedPacket()
    {
        int characterID = BitConverter.ToInt32(_decrypted, 1);
        PlayerAccount.DeleteCharacter(characterID);
    }
    private void HandleCharacterCreatedPacket()
    {
        int index = 1;
        int characterID = BitConverter.ToInt32(_decrypted, index);
        index+=4;
        byte classCode = _decrypted[index];
        index++;
        byte[] statBytes = FillSegment(_decrypted, index, 6);
        index += 6;
        byte[] appearanceBytes = FillSegment(_decrypted, index, 5);
        index += 5;
        byte level = _decrypted[index];
        index++;
        int experience = BitConverter.ToInt32(_decrypted, index);
        index += 4;
        int skills = BitConverter.ToInt32(_decrypted, index);
        index += 4;
        byte[] slots = new byte[10];
        for(int i = 0; i < 10; i++)
        {
            slots[i] = _decrypted[index];
            index++;
        }
        byte nameLength = _decrypted[index];
        index++;
        string characterName = Encoding.UTF8.GetString(_decrypted, index, nameLength);
        PlayerAccount.AddCharacter(characterID, characterName, classCode, level, statBytes, appearanceBytes, slots, skills, 0);
        UICharacterCreationForm creationForm = ComponentRegister.UICharacterCreationForm;
        if (creationForm != null)
        {
            if (!creationForm.gameObject.IsDestroyed())
            {
                creationForm.CloseForm();
                ComponentRegister.UICharacterCreationForm = null;
            }
        }
        SharedFunctions.Params = new object[] { characterID, characterName };
        ComponentRegister.UIPrefabManager.InstantiateAppearanceChooser();
    }
    
    private void HandleLogInSuccessfulPacket()
    {
        Debug.Log("Successful Login.");
        int accountID = BitConverter.ToInt32(_decrypted, 1);
        Game.SetServerTime(BitConverter.ToInt64(_decrypted, 5));
        PlayerAccount.Init(accountID);
        byte tickInterval = _decrypted[13];
        Debug.Log("Tick: " + tickInterval);
        byte pollingFactor = _decrypted[14];
        Game.TickInterval = tickInterval / 1000.0f;
        Game.MovementPolling = Game.TickInterval * pollingFactor;
        Debug.Log("Tick Interval: " + Game.TickInterval);
        Debug.Log("Polling Factor: " + pollingFactor);
        Debug.Log("Movement Polling Interval: " + Game.MovementPolling);
       
        byte characterBytesStart = 15;
        if (_decrypted.Length > characterBytesStart)
        {
            byte numCharacters = _decrypted[characterBytesStart];
            int charIndex = 0;
            int index = characterBytesStart + 1;
            while (charIndex < numCharacters)
            {
                int characterID = BitConverter.ToInt32(_decrypted, index);
                index += 4;
                byte charClass = _decrypted[index];
                index++;
                byte[] statBytes = FillSegment(_decrypted, index, 6);
                index += 6;
                byte[] appearanceBytes = FillSegment(_decrypted, index, 5);
                index += 5;
                Debug.Log("Appearance Bytes - LISP " + characterID );
                for(int i = 0; i < appearanceBytes.Length; i++)
                {
                    Debug.Log(appearanceBytes[i]);
                }
                byte level = _decrypted[index];
                index++;
                /* STRUCTURE
                 * 0 - 3:   ID
                 * 4:       CHARACTER CLASS
                 * 5 - 10:  STATS (STR, DEX, CON, INT, CHA, WIS)
                 * 11 - 15: APPEARANCE (SEX, SKIN, HAIR, FACE, HEAD)
                 * 16:      LEVEL
                 * 17 - 20: EXPERIENCE
                 * 21 - 24: SKILLS
                 * 25 - 34: SLOTS
                 * 35:      NAMELENGTH
                 * 36 - END:NAME
                 */

                int experience = BitConverter.ToInt32(_decrypted, index);
                index += 4;
                int skills = BitConverter.ToInt32(_decrypted, index);
                index += 4;

                byte[] slots = new byte[10];
                for (int i = 0; i < 10; i++)
                {
                    slots[i] = _decrypted[index];
                    index++;
                }

                byte nameLength = _decrypted[index];
                index++;
                string charname = Encoding.UTF8.GetString(_decrypted, index, nameLength);
                index += nameLength;
                PlayerAccount.AddCharacter(characterID, charname, charClass, level, statBytes, appearanceBytes, slots, skills, experience);
                charIndex++;
            }
            
        }
        if(LevelData.LevelCount == 0)
        {
            SendBytes(Pregame_Packets.RequestLevelsListPacket());
        }
        ComponentRegister.UIPrefabManager.InstantiateCharacterSelector();
    }
}
