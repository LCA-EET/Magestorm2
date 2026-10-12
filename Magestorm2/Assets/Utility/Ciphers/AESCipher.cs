using System;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;
using System.Numerics;

public class AESCipher : SymmetricCipher
{
    private static AesCryptoServiceProvider _aesEncryptor, _aesDecryptor;
    private byte[] _ivBytes;
    public AESCipher(Cryptographer owner, byte[] secretSymmetricKey) : base(owner, secretSymmetricKey)
    {
        InitAES(ref _aesEncryptor);
        InitAES(ref _aesDecryptor);
        _ivBytes = new byte[16];
    }

    private void InitAES(ref AesCryptoServiceProvider provider)
    {
        provider = new AesCryptoServiceProvider();
        provider.Padding = PaddingMode.PKCS7;
        provider.Mode = CipherMode.CBC;
    }

    public override byte[] Encrypt(byte[] toEncrypt)
    {
        _iv++;
        BigInteger secretKey = new BigInteger(_secretSymmetricKey, false, true);
        Debug.Log("SecretKey Encryption (Decimal): " + secretKey);
        Debug.Log("SecretKey Encryption (Hex): " + secretKey.ToString("X"));
        ByteUtils.PrintByteArray(_secretSymmetricKey);
        BitConverter.GetBytes(_iv).CopyTo(_ivBytes, 12);
        byte[] encryptedPayload = new byte[toEncrypt.Length];
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                _aesEncryptor.CreateEncryptor(_secretSymmetricKey, _ivBytes), CryptoStreamMode.Write))
            {
                cryptoStream.Write(toEncrypt, 0, toEncrypt.Length);
            }
            encryptedPayload = memoryStream.ToArray();
        }
        byte[] toSend = new byte[encryptedPayload.Length + 2 + 16];
        _ivBytes.CopyTo(toSend, 0);
        byte[] payloadLengthBytes = BitConverter.GetBytes((short)encryptedPayload.Length);
        payloadLengthBytes.CopyTo(toSend, 16);
        encryptedPayload.CopyTo(toSend, 18);
        return toSend;
    }

    public override byte[] Decrypt(byte[] toDecrypt)
    {
        byte[] iv = Packets.IVBytes(toDecrypt);
        byte[] encryptedPayload = Packets.EncryptedPayload(toDecrypt);

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                _aesDecryptor.CreateDecryptor(_secretSymmetricKey, iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedPayload, 0, encryptedPayload.Length);
            }
            return memoryStream.ToArray();
        }
    }
}
