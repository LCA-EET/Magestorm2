import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.GCMParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.math.BigInteger;
import java.security.NoSuchAlgorithmException;

public class AESGCMCipher extends SymmetricCipher{
    private final String _algorithmPlusPadding = "AES/GCM/NoPadding";
    private final String _algorithm = "AES";
    private final byte[] _ivBytes;
    private SecretKey _secretKey;
    private Cipher _aesGCM;
    private final byte[] _receivedIV;
    private static final int _tagLength = 16;
    private static final int _ivLength = 12;

    public AESGCMCipher(Cryptographer owner){
        super(owner);
        _ivBytes = new byte[16];
        _receivedIV = new byte[12];
        try{
            _aesGCM = Cipher.getInstance(_algorithmPlusPadding);
            _secretKeyBytes = GetAESKey(128).getEncoded();
            _secretSymmetricKeyBI = new BigInteger(_secretKeyBytes);
        }
        catch(Exception ex){
            Main.LogError("AESGCMCipher(): " + ex.getMessage());
        }
    }

    public SecretKey GetAESKey(int keysize) throws NoSuchAlgorithmException {
        KeyGenerator keyGen = KeyGenerator.getInstance(_algorithm);
        keyGen.init(keysize, _owner.SecureRandom());
        try{
            _secretKey = keyGen.generateKey();
            Main.LogDebug("AESGCMCipher.GetAESKey successful. Secret key: " + _secretKey.toString());
        }
        catch(Exception e){
            Main.LogError("AESGCMCipher.GetAESKey(): " + e.getMessage());
        }
        return _secretKey;
    }

    @Override
    public byte[] Encrypt(byte[] toEncrypt) {
        _iv++;
        ByteUtils.WriteLongAtIndex_LE(_ivBytes, _iv, 0);
        GCMParameterSpec spec = new GCMParameterSpec(_tagLength * 8, _ivBytes);
        try{
            _aesGCM.init(Cipher.ENCRYPT_MODE, _secretKey, spec);
            int outputSize = _aesGCM.getOutputSize(toEncrypt.length);
            byte[] toReturn = new byte[_ivLength + outputSize];
            System.arraycopy(_ivBytes, 0, toReturn, 0, _ivLength);
            _aesGCM.doFinal(toEncrypt, 0, toEncrypt.length, toReturn, _ivLength);
            return toReturn;
        }
        catch(Exception e){
            Main.LogError("AESGCMCipher.Encrypt(): " + e.getMessage());
        }
        return null;
    }

    @Override
    public byte[] Decrypt(byte[] toDecrypt) {
        if (toDecrypt.length < _ivLength + _tagLength) {
            return null;
        }
        System.arraycopy(toDecrypt, 0, _receivedIV, 0, _ivLength);
        GCMParameterSpec spec = new GCMParameterSpec(_tagLength * 8, _receivedIV);
        try{
            _aesGCM.init(Cipher.DECRYPT_MODE, _secretKey, spec);
            return _aesGCM.doFinal(toDecrypt, _ivLength, toDecrypt.length - _ivLength);
        }
        catch(Exception e){
            Main.LogError("AESGCMCipher.Decrypt(): " + e.getMessage());
        }
        return null;
    }
}
