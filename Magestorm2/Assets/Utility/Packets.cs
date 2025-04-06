using UnityEngine;
using System;

public enum OpCode_Send : byte
{
    LogIn = 1,
    CreateAccount = 2
}
public enum OpCode_Receive : byte
{
    LogInSucceeded = 1,
    LogInFailed = 2,
    AccountCreated = 3,
    AccountCreationFailed = 4
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
}
