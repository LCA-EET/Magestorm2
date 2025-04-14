using UnityEngine;
using System;
using System.Text;

public static class OpCode_Send
{
    public const byte LogIn = 1;
    public const byte CreateAccount = 2;
    public const byte CreateCharacter = 3;
    public const byte LogOut = 4;
}
public enum OpCode_Receive : byte
{
    LogInSucceeded = 1,
    LogInFailed = 2,
    AccountCreated = 3,
    CreationFailed = 4,
    AccountAlreadyExists = 5,
    ProhibitedLanguage = 6,
    AlreadyLoggedIn = 7,
    RemovedFromServer = 8,
    CharacterExists = 9,
    CharacterCreated = 10,
    InactivityDisconnect = 11
}
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

    public static byte[] LogOutPacket()
    {
        byte[] toReturn = new byte[5];
        toReturn[0] = OpCode_Send.LogOut;
        byte[] accountIDBytes = BitConverter.GetBytes(PlayerAccount.AccountID);
        accountIDBytes.CopyTo(toReturn, 1);
        return toReturn;
    }

    public static byte[] CreateCharacterPacket(string charname, byte charclass)
    {
        byte[] idBytes = PlayerAccount.AccountIDBytes;
        byte[] nameBytes = Encoding.UTF8.GetBytes(charname);
        byte nameLength = (byte)nameBytes.Length;
        byte[] toSend = new byte[1 + 4 + 1 + nameLength + 1 + 1];
        toSend[0] = OpCode_Send.CreateCharacter;
        idBytes.CopyTo(toSend, 1);
        nameBytes[5] = nameLength;
        nameBytes.CopyTo(toSend, 6);
        toSend[6 + nameLength] = charclass;
        return toSend;
    }
}
