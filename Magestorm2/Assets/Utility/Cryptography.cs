using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Cryptography
{
    //Mono doesn't include support for AesGcm; using normal AES instead
    private static int _ivSize;
    private static byte[] _key;
    private static long _iv;
    private static byte[] _paddedIV;
    private static AesCryptoServiceProvider _aesEncryptor, _aesDecryptor;
    public static void Init(byte[] key)
    {
        _key = key;
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        _iv = RandomNumberGenerator.GetInt32(Int32.MaxValue);
        _ivSize = 16;
        _paddedIV = new byte[16];
        InitAES(ref _aesEncryptor);
        InitAES(ref _aesDecryptor);
    }
    private static void InitAES(ref AesCryptoServiceProvider provider)
    {
        provider = new AesCryptoServiceProvider();
        provider.Padding = PaddingMode.PKCS7;
        provider.Mode = CipherMode.CBC;
    }
    public static void EncryptAndSend(byte[] payload, UDPGameClient udp)
    {
        _iv++; // not ideal from a security perspective, but it's a lot less expensive than generating a new random nonce for each packet
        byte[] ivBytes = PadBytes(8, _iv);

        byte[] encryptedPayload = new byte[payload.Length];

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _aesEncryptor.CreateEncryptor(_key, ivBytes), CryptoStreamMode.Write))
            {
                cryptoStream.Write(payload, 0, payload.Length);
            }
            encryptedPayload = memoryStream.ToArray();
        }
        byte[] toSend = new byte[encryptedPayload.Length + 1 + _ivSize];
        ivBytes.CopyTo(toSend, 0);
        Debug.Log("IV64: " + Convert.ToBase64String(ivBytes));
        toSend[_ivSize] = (byte)encryptedPayload.Length;
        encryptedPayload.CopyTo(toSend, _ivSize + 1);
        Debug.Log("Encrypted Payload: " + Convert.ToBase64String(encryptedPayload));
        Debug.Log("Packet length: " + toSend.Length);
        udp.Send(toSend);
    }
    public static byte[] DecryptReceived(byte[] received)
    {
        byte[] iv = Packets.IVBytes(received);
        byte[] encryptedPayload = Packets.EncryptedPayload(received);

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _aesDecryptor.CreateDecryptor(_key, iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedPayload, 0, encryptedPayload.Length);
            }
            return memoryStream.ToArray();
        }
    }
    private static byte[] PadBytes(byte additionalBytes, long nonce)
    {
        BitConverter.GetBytes(nonce).CopyTo(_paddedIV, 8);
        return _paddedIV;
    }

}
