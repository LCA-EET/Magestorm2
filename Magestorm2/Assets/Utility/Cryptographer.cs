using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
public class Cryptographer
{
    private static RandomNumberGenerator _random;
    private static SHA256 _sha256;
    public static BigInteger ChecksumShort = new BigInteger(short.MaxValue);
    private static Dictionary<string, byte[]> _storedSymmetricKeys;
    private SymmetricCipher _cipher;
    
    public Cryptographer(byte cipherType, byte[] secretKey)
    {
        _random = RandomNumberGenerator.Create();
        switch (cipherType)
        {
            case ControlCodes.CipherCode_AES:
                _cipher = new AESCipher(this, secretKey);
                break;
            default:
                _cipher = new SymmetricCipher(this, secretKey);
                break;
        }
    }
    public static bool GetStoredSecretKey(string keyID, out byte[] value)
    {
        if (_storedSymmetricKeys == null)
        {
            _storedSymmetricKeys = new Dictionary<string, byte[]>();
        }
        else
        {
            string lower = keyID.ToLower();
            if (_storedSymmetricKeys.ContainsKey(lower))
            {
                value = _storedSymmetricKeys[lower];
                return true;
            }
        }
        value = null;
        return false;
    }
    public static bool VerifyChecksum(byte[] payload, int checksumIndex)
    {
        short checksum = BitConverter.ToInt16(payload, checksumIndex);
        byte[] shortened = new byte[checksumIndex];
        Array.Copy(payload, 0, shortened, 0, shortened.Length);
        BigInteger sum = new BigInteger(shortened, false, true);
        return ModChecksum(sum) == checksum;
    }
    public static void StoreKey(string keyID, byte[] value)
    {
        if (_storedSymmetricKeys == null)
        {
            _storedSymmetricKeys = new Dictionary<string, byte[]>();
        }
        _storedSymmetricKeys.Add(keyID.ToLower(), value);
    }

    public static short ModChecksum(BigInteger dividend)
    {
        BigInteger calculatedRemainder;
        BigInteger.DivRem(dividend, ChecksumShort, out calculatedRemainder);
        Debug.Log("ModChecksum: " +  calculatedRemainder);
        return (short)calculatedRemainder;
    }

    public static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
    {
        BigInteger range = max - min + 1; // inclusive range
        int byteCount = (int)Math.Ceiling(BigInteger.Log(range, 256));

        byte[] bytes = new byte[byteCount];

        while (true)
        {
            _random.GetBytes(bytes);

            // BigInteger in .NET expects little-endian, so reverse for big-endian
            Array.Reverse(bytes);

            // Ensure positive number by appending a zero byte if needed
            byte[] littleEndian = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, littleEndian, 0, bytes.Length);

            BigInteger value = new BigInteger(littleEndian);

            if (value < range)
                return min + value;
        }
    }
    public static BigInteger First16Bytes(BigInteger value)
    {
        byte[] bytes = value.ToByteArray();
        Array.Reverse(bytes); 
        byte[] first16 = new byte[16];
        System.Array.Copy(bytes, 0, first16, 0, 16);
        return new BigInteger(first16, false, true);
    }
    public static byte[] ToBigEndianBytes(BigInteger value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Only non-negative values supported.");

        byte[] littleEndian = value.ToByteArray(isUnsigned: false, isBigEndian: false);
        Array.Reverse(littleEndian);
        return littleEndian;
    }

    
    
    
    public static string SHA256Hash(string toHash)
    {
        if(_sha256 == null)
        {
            _sha256 = SHA256.Create();
        }
        byte[] utf8 = Encoding.UTF8.GetBytes(toHash);
        byte[] hashed = _sha256.ComputeHash(utf8);
        return Convert.ToBase64String(hashed);
    }

    public byte[] Encrypt(byte[] toEncrypt)
    {
        return _cipher.Encrypt(toEncrypt);
    }

    public byte[] Decrypt(byte[] toDecrypt)
    {
        return _cipher.Decrypt(toDecrypt);
    }
}
