using UnityEngine;
using System;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;

public static class Packets
{

    public static byte DeterminePayloadLength(byte[] received)
    {
        return received[16];
    }

    public static byte[] IVBytes(byte[] received)
    {
        return new ArraySegment<byte>(received, 0, 16).ToArray();
    }

    public static byte[] EncryptedPayload(byte[] received)
    {
        return new ArraySegment<byte>(received, 17, DeterminePayloadLength(received)).ToArray();
    }
    public static byte[] UpdateAppearancePacket(byte[] appearaneBytes, int characterID)
    {
        byte[] unencrypted = new byte[14];
        unencrypted[0] = OpCode_Send.UpdateAppearance;
        PlayerAccount.AccountIDBytes.CopyTo(unencrypted, 1);
        BitConverter.GetBytes(characterID).CopyTo(unencrypted, 5);
        appearaneBytes.CopyTo(unencrypted, 9);
        return unencrypted;
    }
    public static byte[] NameCheckPacket(string name)
    { 
        byte[] nameBytes = ByteUtils.UTF8ToBytes(name);
        byte nameLength = (byte)nameBytes.Length;
        byte[] unencrypted = new byte[6 + nameLength];
        unencrypted[0] = OpCode_Send.NameCheck;
        PlayerAccount.AccountIDBytes.CopyTo(unencrypted, 1);
        unencrypted[5] = nameLength;
        nameBytes.CopyTo(unencrypted, 6);
        return unencrypted;
    }
    public static byte[] MatchDetailsPacket(MatchEntry matchDetails)
    {
        byte[] unencrypted = new byte[6];
        unencrypted[0] = OpCode_Send.RequestMatchDetails;
        PlayerAccount.AccountIDBytes.CopyTo(unencrypted, 1);
        unencrypted[5] = matchDetails.MatchID;
        return unencrypted;
    }
    public static byte[] CreateAccountPacket(string username, string hashedPassword, string email)
    {
        byte[] usernameBytes = UTF8Encoding.UTF8.GetBytes(username);
        byte[] hashedBytes = Convert.FromBase64String(hashedPassword);
        byte[] emailBytes = UTF8Encoding.UTF8.GetBytes(email);

        byte[] unencryptedPayload = new byte[usernameBytes.Length + hashedBytes.Length + emailBytes.Length + 4];

        unencryptedPayload[0] = OpCode_Send.CreateAccount;
        unencryptedPayload[1] = (byte)usernameBytes.Length;
        unencryptedPayload[2] = (byte)hashedBytes.Length;
        unencryptedPayload[3] = (byte)emailBytes.Length;

        usernameBytes.CopyTo(unencryptedPayload, 4);
        hashedBytes.CopyTo(unencryptedPayload, 4 + usernameBytes.Length);
        emailBytes.CopyTo(unencryptedPayload, 4 + usernameBytes.Length + hashedBytes.Length);

        return unencryptedPayload;
    }
    public static byte[] LogInPacket(string username, string hashedPassword)
    {
        byte[] usernameBytes = UTF8Encoding.UTF8.GetBytes(username);
        byte[] hashedBytes = Convert.FromBase64String(hashedPassword);

        byte[] unencryptedPayload = new byte[usernameBytes.Length + hashedBytes.Length + 3];
        unencryptedPayload[0] = OpCode_Send.LogIn;
        unencryptedPayload[1] = (byte)usernameBytes.Length;
        unencryptedPayload[2] = (byte)hashedBytes.Length;
        usernameBytes.CopyTo(unencryptedPayload, 3);
        hashedBytes.CopyTo(unencryptedPayload, 3 + usernameBytes.Length);

        return unencryptedPayload;
    }
    public static byte[] RequestLevelsListPacket()
    {
        return OpCodePlusAccountIDBytes(OpCode_Send.RequestLevelsList);
    }
    public static byte[] DeleteMatchPacket()
    {
        return OpCodePlusAccountIDBytes(OpCode_Send.DeleteMatch);
    }
    public static byte[] CreateMatchPacket(byte sceneID)
    {
        byte[] idBytes = PlayerAccount.AccountIDBytes;
        byte[] toSend = new byte[1 + 4 + 1];
        toSend[0] = OpCode_Send.CreateMatch;
        idBytes.CopyTo(toSend, 1);
        toSend[5] = sceneID;
        return toSend;
    }

    public static byte[] CreateCharacterPacket(string charname, byte charclass, byte[] stats, byte[] appearance)
    {
        byte[] idBytes = PlayerAccount.AccountIDBytes;
        byte[] nameBytes = Encoding.UTF8.GetBytes(charname);
        byte nameLength = (byte)nameBytes.Length;
        byte[] toSend = new byte[1 + 4 + 1 + 6 + 5 + 1 + nameLength];
        toSend[0] = OpCode_Send.CreateCharacter;
        idBytes.CopyTo(toSend, 1);
        toSend[5] = charclass;
        stats.CopyTo(toSend, 6);
        appearance.CopyTo(toSend, 12);
        toSend[17] = nameLength;
        nameBytes.CopyTo(toSend, 18);
        return toSend;
    }

    public static byte[] DeleteCharacterPacket(int characterID)
    {
        byte[] characterIDbytes = BitConverter.GetBytes(characterID);
        byte[] accountIDbytes = PlayerAccount.AccountIDBytes;
        byte[] toSend = new byte[1 + 4 + 4];
        toSend[0] = OpCode_Send.DeleteCharacter;
        accountIDbytes.CopyTo(toSend, 1);
        characterIDbytes.CopyTo(toSend, 5);
        return toSend;
    }
    #region OpCodePlusAccountID
    public static byte[] LogOutPacket()
    {
        return OpCodePlusAccountIDBytes(OpCode_Send.LogOut);
    }
    public static byte[] UnsubscribeFromMatchesPacket()
    {
        return OpCodePlusAccountIDBytes(OpCode_Send.UnsubscribeFromMatches);
    }
    public static byte[] SubscribeToMatchesPacket()
    {
        byte[] toSend = new byte[1 + 4 + 4];
        toSend[0] = OpCode_Send.SubscribeToMatches;
        PlayerAccount.AccountIDBytes.CopyTo(toSend, 1);
        PlayerAccount.SelectedCharacter.IDBytes.CopyTo(toSend, 5);
        return toSend;
    }
    public static byte[] OpCodePlusAccountIDBytes(byte opCode)
    {
        byte[] toSend = new byte[1 + 4];
        toSend[0] = opCode;
        PlayerAccount.AccountIDBytes.CopyTo(toSend, 1);
        return toSend;
    }
    #endregion


}
