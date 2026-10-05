using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
public static class Cryptography
{
    //Mono doesn't include support for AesGcm; using normal AES instead
    private static int _ivSize;
    private static byte[] _key;
    private static long _iv;
    private static byte[] _paddedIV;
    private static AesCryptoServiceProvider _aesEncryptor, _aesDecryptor;
    private static SHA256 _sha256;
    public static BigInteger ChecksumShort = new BigInteger(short.MaxValue);
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
    public static short ModChecksum(BigInteger dividend)
    {
        BigInteger calculatedRemainder;
        BigInteger.DivRem(dividend, ChecksumShort, out calculatedRemainder);
        return (short)calculatedRemainder;
    }
    public static byte[] MD5Digest(byte[] toHash)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(toHash);
            return hashBytes;
        }
    }
    public static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
    {
        BigInteger range = max - min + 1; // inclusive range
        int byteCount = (int)Math.Ceiling(BigInteger.Log(range, 256));

        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[byteCount];

            while (true)
            {
                rng.GetBytes(bytes);

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

    private static void InitAES(ref AesCryptoServiceProvider provider)
    {
        provider = new AesCryptoServiceProvider();
        provider.Padding = PaddingMode.PKCS7;
        provider.Mode = CipherMode.CBC;
    }
    public static void EncryptAndSend(byte[] payload)
    {
        if (Game.SymmetricEncryption)
        {
            _iv++; 
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
            toSend[_ivSize] = (byte)encryptedPayload.Length;
            encryptedPayload.CopyTo(toSend, _ivSize + 1);
            Game.UDP.Send(toSend);
        }
        else
        {
            Game.UDP.Send(payload);
        }
    }
    public static byte[] DecryptReceived(byte[] received)
    {
        if (Game.SymmetricEncryption)
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
        else
        {
            return received;
        }
    }
    private static byte[] PadBytes(byte additionalBytes, long nonce)
    {
        BitConverter.GetBytes(nonce).CopyTo(_paddedIV, 8);
        return _paddedIV;
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


}
