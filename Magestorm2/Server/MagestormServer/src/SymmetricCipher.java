import java.math.BigInteger;

public class SymmetricCipher{
    protected byte[] _secretKeyBytes;
    protected BigInteger _secretSymmetricKeyBI;
    protected int _iv;
    protected Cryptographer _owner;
    public SymmetricCipher(Cryptographer owner){
        _owner = owner;
        _iv = _owner.SecureRandom().nextInt();
        _secretSymmetricKeyBI = BigInteger.ZERO;
        _secretKeyBytes = _secretSymmetricKeyBI.toByteArray();
    }

    public BigInteger GetSecretSymmetricKeyBI(){
        return _secretSymmetricKeyBI;
    }

    public byte[] GetSecretSymmetricKeyBytes(){
        return _secretKeyBytes;
    }

    public byte[] Encrypt(byte[] toEncrypt){
        return toEncrypt;
    }

    public byte[] Decrypt(byte[] toDecrypt){
        return toDecrypt;
    }
}
