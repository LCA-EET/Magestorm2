using System;

public static class Packets
{
    public static short DeterminePayloadLength(byte[] received)
    {
        return BitConverter.ToInt16(received, 16);
    }

    public static byte[] IVBytes(byte[] received)
    {
        return new ArraySegment<byte>(received, 0, 16).ToArray();
    }

    public static byte[] EncryptedPayload(byte[] received)
    {
        return new ArraySegment<byte>(received, 18, DeterminePayloadLength(received)).ToArray();
    }

}
