using System;
using System.Security.Cryptography;

public class SymmetricCipher
{
    protected byte[] _secretSymmetricKey;
    protected int _iv;
    protected Cryptographer _owner;
    public SymmetricCipher(Cryptographer owner, byte[] secretSymmetricKey)
    {
        _owner = owner;
        _secretSymmetricKey = secretSymmetricKey;
        _iv = RandomNumberGenerator.GetInt32(Int32.MaxValue);
    }
    public virtual byte[] Encrypt(byte[] toEncrypt)
    {
        return toEncrypt;
    }
    public virtual byte[] Decrypt(byte[] toDecrypt)
    {
        return toDecrypt;
    }
}
