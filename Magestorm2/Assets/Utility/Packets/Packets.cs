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

}
