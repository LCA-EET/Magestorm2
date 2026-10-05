using System;
using System.Numerics;

public class DHKE_Packets
{
    public static byte[] DPRequestPacket()
    {
        return new byte[] { DHKE_Send.DPRequest };
    }

    public static byte[] ClientPublicKeyPacket(BigInteger kpub)
    {
        byte[] kpubBytes = Cryptography.ToBigEndianBytes(kpub);
        byte[] lengthBytes = BitConverter.GetBytes((short)kpubBytes.Length);
        byte[] toReturn = new byte[1 + 2 + kpubBytes.Length + 2];
        toReturn[0] = DHKE_Send.ClientPublicKey;
        Array.Copy(lengthBytes, 0, toReturn, 1, 2);
        Array.Copy(kpubBytes, 0, toReturn, 3, kpubBytes.Length);
        byte[] remainder = BitConverter.GetBytes(Cryptography.ModChecksum(kpub));
        Array.Copy(remainder, 0, toReturn, 3 + kpubBytes.Length, 2);
        return toReturn;
    }
}
