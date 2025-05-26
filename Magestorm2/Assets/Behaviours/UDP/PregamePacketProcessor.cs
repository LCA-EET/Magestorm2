using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class PregamePacketProcessor : MonoBehaviour
{
    private int _listeningPort;
    private UDPGameClient _udp;
    private bool _checking;

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
            if (!_checking)
            {
                _checking = true;
            }
            if (_udp.HasPacketsPending)
            {
                List<byte[]> toProcess = _udp.PacketsReceived();
                foreach (byte[] decryptedPayload in toProcess)
                {
                    OpCode_Receive opCode = (OpCode_Receive)decryptedPayload[0];
                    Debug.Log("OpCode Received: " + opCode);
                    switch (opCode)
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
                            HandleLogInSuccessfulPacket(decryptedPayload);
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
                            HandleCharacterCreatedPacket(decryptedPayload);
                            break;
                        case OpCode_Receive.CharacterDeleted:
                            HandleCharacterDeletedPacket(decryptedPayload);
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
                            HandleMatchDataPacket(decryptedPayload);
                            break;
                        case OpCode_Receive.LevelsList:
                            HandleLevelListPacket(decryptedPayload);
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
                            HandleMatchDetailsPacket(decryptedPayload);
                            break;
                        case OpCode_Receive.NameCheckResult:
                            HandleNameCheckResultPacket(decryptedPayload);
                            break;
                    }
                }
            }
        }
    }
    private void HandleNameCheckResultPacket(byte[] decrypted)
    {
        byte isUsed = decrypted[1];
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
    private void HandleMatchDetailsPacket(byte[] decrypted)
    {
        
        byte matchID = decrypted[1];
        int index = 2;
        RemotePlayerData[] neutralPlayers = ProcessMatchPlayers(ref index, decrypted);
        RemotePlayerData[] balancePlayers = ProcessMatchPlayers(ref index, decrypted);
        RemotePlayerData[] chaosPlayers = ProcessMatchPlayers(ref index, decrypted);
        RemotePlayerData[] orderPlayers = ProcessMatchPlayers(ref index, decrypted);
        ListedMatch match = null;
        if(ActiveMatches.GetMatch(matchID, ref match))
        {
            SharedFunctions.Params = new object[] { match, chaosPlayers, balancePlayers, orderPlayers };
            ComponentRegister.UIPrefabManager.InstantiateJoinMatch();
        }      
    }
    private RemotePlayerData[] ProcessMatchPlayers(ref int index, byte[] decrypted)
    {
        byte numPlayers = decrypted[index];
        RemotePlayerData[] toReturn = new RemotePlayerData[numPlayers];
        index++;
        int playerIndex = 0;
        while (playerIndex < numPlayers)
        {
            byte idInMatch = decrypted[index];
            index++;
            byte teamID = decrypted[index];
            index++;
            byte playerClass = decrypted[index];
            index++;
            byte playerLevel = decrypted[index];
            index++;
            byte nameLength = decrypted[index];
            index++;
            string playerName = ByteUtils.BytesToUTF8(decrypted, index, nameLength);
            index += nameLength;
            toReturn[playerIndex] = new RemotePlayerData(idInMatch, teamID, playerName, playerLevel, (PlayerClass)playerClass);
            playerIndex++;
        }
        return toReturn;
    }
    private void HandleLevelListPacket(byte[] decrypted)
    {
        byte numLevels = decrypted[1];
        byte levelIdx = 0;
        int index = 2;
        while(levelIdx < numLevels)
        {
            byte sceneID = decrypted[index];
            index++;
            byte maxPlayers = decrypted[index];
            index++;
            byte nameLength = decrypted[index];
            index++;
            byte[] nameBytes = new byte[nameLength];
            Array.Copy(decrypted, index, nameBytes, 0, nameLength);
            index += nameLength;
            levelIdx++;
            LevelData.AddLevel(sceneID, maxPlayers, Encoding.UTF8.GetString(nameBytes));
        }
    }
    private void HandleMatchDataPacket(byte[] decrypted)
    {
        byte matchCount = decrypted[1];
        int index = 2;
        ActiveMatches.ClearMatches();
        for(int i = 0; i < matchCount; i++)
        {
            byte matchID = decrypted[index];
            index++;
            byte sceneID = decrypted[index];
            index++;
            long expirationTime = BitConverter.ToInt64(decrypted, index);
            long currentTime = TimeUtil.CurrentTime();
            index += 8;
            int creatorAccountID = BitConverter.ToInt32(decrypted, index);
            index += 4;
            byte nameLength = decrypted[index];
            byte[] nameBytes = new byte[nameLength];
            index++;
            Array.Copy(decrypted, index, nameBytes, 0, nameLength);
            string creatorName = Encoding.UTF8.GetString(nameBytes);
            index += nameLength;
            ListedMatch toAdd = new ListedMatch(matchID, sceneID, creatorName, expirationTime, creatorAccountID);
            ActiveMatches.AddMatch(toAdd);
        }
    }
    public void SendBytes(byte[] unencrypted)
    {
        Cryptography.EncryptAndSend(unencrypted, _udp);
    }
    private void MessageBox(int stringReference)
    {
        Game.MessageBox(Language.GetBaseString(stringReference));
    }
    public void Init(int port)
    {
        _listeningPort = port;
        _udp = UDPBuilder.GetClient(port);
    }
    private void HandleCharacterDeletedPacket(byte[] decryptedPayload)
    {
        int characterID = BitConverter.ToInt32(decryptedPayload, 1);
        PlayerAccount.DeleteCharacter(characterID);
    }
    private void HandleCharacterCreatedPacket(byte[] decrypted)
    {
        byte classCode = decrypted[1];
        int characterID = BitConverter.ToInt32(decrypted, 2);
        byte[] appearanceBytes = FillSegment(decrypted, 6, 5);
        byte[] statBytes = FillSegment(decrypted, 11, 6);
        byte nameLength = decrypted[17];
        string characterName = Encoding.UTF8.GetString(decrypted, 18, nameLength);
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
    
    private byte[] FillSegment(byte[] source, int sourceIndex, int length)
    {
        byte[] statBytes = new byte[length];
        Array.Copy(source, sourceIndex, statBytes, 0, length);
        return statBytes;
    }
    private void HandleLogInSuccessfulPacket(byte[] decrypted)
    {
        int accountID = BitConverter.ToInt32(decrypted, 1);
        Game.SetServerTime(BitConverter.ToInt64(decrypted, 5));
        PlayerAccount.Init(accountID);
        byte characterBytesStart = 13;
        if (decrypted.Length > characterBytesStart)
        {
            byte numCharacters = decrypted[characterBytesStart];
            int charIndex = 0;
            int index = characterBytesStart + 1;
            while (charIndex < numCharacters)
            {
                int characterID = BitConverter.ToInt32(decrypted, index);
                index += 4;
                byte charClass = decrypted[index];
                index++;
                byte[] statBytes = FillSegment(decrypted, index, 6);
                index += 6;
                byte[] appearanceBytes = FillSegment(decrypted, index, 5);
                index += 5;
                byte level = decrypted[index];
                index++;
                int experience = BitConverter.ToInt32(decrypted, index);
                index += 4;
                byte nameLength = decrypted[index];
                index++;
                string charname = Encoding.UTF8.GetString(decrypted, index, nameLength);
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
