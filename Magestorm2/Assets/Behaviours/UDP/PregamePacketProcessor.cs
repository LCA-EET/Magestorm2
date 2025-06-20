using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PregamePacketProcessor : UDPProcessor
{

    private void Awake()
    {
        ComponentRegister.PregamePacketProcessor = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_listeningPort > 0)
        {
            if (_udp.HasPacketsPending)
            {
                List<byte[]> toProcess = _udp.PacketsReceived();
                foreach (byte[] decryptedPayload in toProcess)
                {
                    PreProcess(decryptedPayload);
                    switch (_opCode)
                    {
                        case OpCode_Receive.CreationFailed:
                            MessageBox(26);
                            break;
                        case OpCode_Receive.AccountCreated:
                            MessageBox(24);
                            break;
                        case OpCode_Receive.AccountAlreadyExists:
                            MessageBox(25);
                            break;
                        case OpCode_Receive.LogInFailed:
                            MessageBox(27);
                            break;
                        case OpCode_Receive.LogInSucceeded:
                            HandleLogInSuccessfulPacket();
                            break;
                        case OpCode_Receive.ProhibitedLanguage:
                            MessageBox(29);
                            break;
                        case OpCode_Receive.AlreadyLoggedIn:
                            MessageBox(30);
                            break;
                        case OpCode_Receive.RemovedFromServer:
                        case OpCode_Receive.InactivityDisconnect:
                            MessageBox(31);
                            Game.Quit();
                            break;
                        case OpCode_Receive.CharacterExists:
                            MessageBox(33);
                            break;
                        case OpCode_Receive.CharacterCreated:
                            HandleCharacterCreatedPacket();
                            break;
                        case OpCode_Receive.CharacterDeleted:
                            HandleCharacterDeletedPacket();
                            break;
                        case OpCode_Receive.MatchStillHasPlayers:
                            MessageBox(48);
                            break;
                        case OpCode_Receive.MatchLimitReached:
                            MessageBox(46);
                            break;
                        case OpCode_Receive.MatchAlreadyCreated:
                            MessageBox(45);
                            break;
                        case OpCode_Receive.MatchData:
                            HandleMatchDataPacket();
                            break;
                        case OpCode_Receive.LevelsList:
                            HandleLevelListPacket();
                            break;
                        case OpCode_Receive.BannedForBehavior:
                            MessageBox(70);
                            Game.Quit();
                            break;
                        case OpCode_Receive.BannedForCheating:
                            MessageBox(69);
                            Game.Quit();
                            break;
                        case OpCode_Receive.MatchDetails:
                            HandleMatchDetailsPacket();
                            break;
                        case OpCode_Receive.NameCheckResult:
                            HandleNameCheckResultPacket();
                            break;
                        case OpCode_Receive.MatchIsFullPacket:
                            HandleMatchIsFullPacket();
                            break;
                        case OpCode_Receive.MatchEntryPacket:
                            HandleMatchEntryPacket();
                            break;

                    }
                }
            }
        }
    }
    private void HandleMatchEntryPacket()
    {
        MatchParams.Init(_decrypted);
        SceneManager.LoadScene(MatchParams.SceneID.ToString());
    }
    private void HandleMatchIsFullPacket()
    {
        Game.MessageBoxReference(100);
    }
    private void HandleNameCheckResultPacket()
    {
        byte isUsed = _decrypted[1];
        switch (isUsed)
        {
            case 0:
                ComponentRegister.UICharacterCreationForm.NameCheckPassed();
                break;
            case 1:
                Game.MessageBoxReference(95);
                break;
            case 2:
                Game.MessageBoxReference(96);
                break;
        }
    }
    private void HandleMatchDetailsPacket()
    {
        
        byte matchID = _decrypted[1];
        int index = 2;
        RemotePlayerData[] neutralPlayers = ProcessMatchPlayers(ref index);
        RemotePlayerData[] balancePlayers = ProcessMatchPlayers(ref index);
        RemotePlayerData[] chaosPlayers = ProcessMatchPlayers(ref index);
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
        RemotePlayerData[] toReturn = new RemotePlayerData[numPlayers];
        index++;
        int playerIndex = 0;
        while (playerIndex < numPlayers)
        {
            byte idInMatch = _decrypted[index];
            index++;
            byte teamID = _decrypted[index];
            index++;
            byte playerClass = _decrypted[index];
            index++;
            byte playerLevel = _decrypted[index];
            index++;
            byte nameLength = _decrypted[index];
            index++;
            string playerName = ByteUtils.BytesToUTF8(_decrypted, index, nameLength);
            index += nameLength;
            toReturn[playerIndex] = new RemotePlayerData(idInMatch, teamID, playerName, playerLevel, (PlayerClass)playerClass);
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
            byte nameLength = _decrypted[index];
            index++;
            byte[] nameBytes = new byte[nameLength];
            Array.Copy(_decrypted, index, nameBytes, 0, nameLength);
            index += nameLength;
            levelIdx++;
            LevelData.AddLevel(sceneID, maxPlayers, Encoding.UTF8.GetString(nameBytes));
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
            Array.Copy(_decrypted, index, nameBytes, 0, nameLength);
            string creatorName = Encoding.UTF8.GetString(nameBytes);
            index += nameLength;
            ListedMatch toAdd = new ListedMatch(matchID, sceneID, creatorName, expirationTime, creatorAccountID);
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
        byte classCode = _decrypted[1];
        int characterID = BitConverter.ToInt32(_decrypted, 2);
        byte[] appearanceBytes = FillSegment(_decrypted, 6, 5);
        byte[] statBytes = FillSegment(_decrypted, 11, 6);
        byte nameLength = _decrypted[17];
        string characterName = Encoding.UTF8.GetString(_decrypted, 18, nameLength);
        PlayerAccount.AddCharacter(characterID, characterName, classCode, 1, statBytes, appearanceBytes);
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
        int accountID = BitConverter.ToInt32(_decrypted, 1);
        Game.SetServerTime(BitConverter.ToInt64(_decrypted, 5));
        PlayerAccount.Init(accountID);
        byte characterBytesStart = 13;
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
                byte level = _decrypted[index];
                index++;
                int experience = BitConverter.ToInt32(_decrypted, index);
                index += 4;
                byte nameLength = _decrypted[index];
                index++;
                string charname = Encoding.UTF8.GetString(_decrypted, index, nameLength);
                index += nameLength;
                PlayerAccount.AddCharacter(characterID, charname, charClass, level, statBytes, appearanceBytes);
                charIndex++;
            }
            
        }
        if(LevelData.LevelCount == 0)
        {
            SendBytes(Packets.RequestLevelsListPacket());
        }
        ComponentRegister.UIPrefabManager.InstantiateCharacterSelector();
    }
}
