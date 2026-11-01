using System;
using System.Security.Cryptography;
using System.Buffers.Binary;
public class AESGCMCipher : SymmetricCipher
{
    private byte[] _ivBytes;
    private int _ivPlusTagSize, _ivLength, _tagLength;
    private AesGcm _aesGCM;
    public AESGCMCipher(Cryptographer owner, byte[] secretSymmetricKey) : base(owner, secretSymmetricKey)
    {
        _ivLength = AesGcm.NonceByteSizes.MaxSize;
        _tagLength = AesGcm.TagByteSizes.MaxSize;
        _ivPlusTagSize = _ivLength + _tagLength;
        _ivBytes = new byte[_ivLength];
        RandomNumberGenerator.Fill(_ivBytes);
        _aesGCM = new AesGcm(secretSymmetricKey);
    }

    public override byte[] Encrypt(byte[] toEncrypt)
    {
        _iv++;
        BinaryPrimitives.WriteInt32LittleEndian(_ivBytes.AsSpan(8), _iv);
        byte[] packet = new byte[_ivPlusTagSize + toEncrypt.Length];
        _ivBytes.CopyTo(packet, 0);
        Span<byte> cipherText = packet.AsSpan(_ivLength, toEncrypt.Length);
        Span<byte> tag = packet.AsSpan(_ivLength + toEncrypt.Length, _tagLength);
        _aesGCM.Encrypt(_ivBytes, toEncrypt, cipherText, tag);
        return packet;
    }

    public override byte[] Decrypt(byte[] packet)
    {
        int cipherTextLength = packet.Length - _ivPlusTagSize;
        byte[] plaintext = new byte[cipherTextLength];
        try
        {
            _aesGCM.Decrypt(
                packet.AsSpan(0, _ivLength),
                packet.AsSpan(_iv, cipherTextLength),
                packet.AsSpan(_ivLength + cipherTextLength, _tagLength),
                plaintext);

            return plaintext;
        }
        catch (CryptographicException)
        {
            return Array.Empty<byte>();
        }
    }
}
