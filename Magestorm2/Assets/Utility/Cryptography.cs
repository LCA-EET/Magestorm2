using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public static class Cryptography
{
    //Mono doesn't include support for AesGcm; using normal AES instead
    private static int _ivSize;
    private static byte[] _key;
    private static long _iv;
    private static byte[] _paddedIV;
    public static void InitGCM(byte[] key)
    {
        _key = key;
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        _iv = RandomNumberGenerator.GetInt32(Int32.MaxValue);
        _ivSize = 16;
        _paddedIV = new byte[16];
    }
    public static void EncryptAndSend(byte[] payload, UDPGameClient udp)
    {
        _iv++; // not ideal from a security perspective, but it's a lot less expensive than generating a new random nonce for each packet
        byte[] ivBytes = PadBytes(8, _iv);

        byte[] encryptedPayload = new byte[payload.Length];

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesProvider.CreateEncryptor(_key, ivBytes), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(payload, 0, payload.Length);
                }
            }
            encryptedPayload = memoryStream.ToArray();
        }
        byte[] toSend = new byte[encryptedPayload.Length + 1 + _ivSize];
        ivBytes.CopyTo(toSend, 0);
        toSend[_ivSize] = (byte)encryptedPayload.Length;
        encryptedPayload.CopyTo(toSend, _ivSize + 1);
        udp.Send(toSend);
    }
    private static byte[] PadBytes(byte additionalBytes, long nonce)
    {
        BitConverter.GetBytes(nonce).CopyTo(_paddedIV, 8);
        return _paddedIV;
    }

}
