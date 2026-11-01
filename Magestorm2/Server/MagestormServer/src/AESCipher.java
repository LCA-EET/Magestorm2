import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.math.BigInteger;
import java.security.NoSuchAlgorithmException;

public class AESCipher extends SymmetricCipher{

    private SecretKeySpec _keySpec;
    private SecretKey _secretKey;
    private Cipher _decryptionCipher, _encryptionCipher;
    private final String _algorithmPlusPadding = "AES/CBC/PKCS5Padding";
    private final String _algorithm = "AES";
    private final byte[] _ivBytes;
    public AESCipher(Cryptographer owner){
        super(owner);
        _ivBytes = new byte[16];
        try{
            _secretKeyBytes = GetAESKey(128).getEncoded();
            _secretSymmetricKeyBI = new BigInteger(_secretKeyBytes);
        }
        catch(Exception ex){
            Main.LogError("Exception in AESCipher() " + ex.getMessage());
        }
    }

    @Override
    public byte[] Encrypt(byte[] payload) {
        try {
            _iv++;
            ByteUtils.WriteIntAtIndex_LE(_ivBytes, _iv, 0);
            IvParameterSpec iv = new IvParameterSpec(_ivBytes);
            _encryptionCipher.init(Cipher.ENCRYPT_MODE, _secretKey, iv);
            byte[] result = new byte[18 + _encryptionCipher.getOutputSize(payload.length)];
            System.arraycopy(_ivBytes, 0, result, 0, 16);
            int actualLength =
                    _encryptionCipher.doFinal(payload, 0, payload.length,
                            result, 18);
            ByteUtils.WriteShortAtIndex_LE(result, (short) actualLength, 16);
            return result;
        }
        catch (Exception e) {
            Main.LogError("AESCipher.Encrypt(): " + e.getMessage());
            return null;
        }
    }

    @Override
    public byte[] Decrypt(byte[] toDecrypt){
        byte[] ivBytes = new byte[16];
        short payloadLength = ByteUtils.ExtractShort(toDecrypt, 16);
        System.arraycopy(toDecrypt,0,ivBytes,0,ivBytes.length);
        try {
            IvParameterSpec iv = new IvParameterSpec(ivBytes);
            _decryptionCipher.init(Cipher.DECRYPT_MODE, _secretKey, iv);
            return _decryptionCipher.doFinal(toDecrypt, 18, payloadLength);
        } catch (Exception e) {
            Main.LogError("AESCipher.Decrypt(): Decryption error: " + e.getMessage());
        }
        return new byte[0];
    }
    public SecretKey GetAESKey(int keysize) throws NoSuchAlgorithmException {
        KeyGenerator keyGen = KeyGenerator.getInstance(_algorithm);
        keyGen.init(keysize, _owner.SecureRandom());
        try{
            _secretKey = keyGen.generateKey();
            _keySpec = new SecretKeySpec(_secretKey.getEncoded(), _algorithm);
            _decryptionCipher = Cipher.getInstance(_algorithmPlusPadding);
            _encryptionCipher = Cipher.getInstance(_algorithmPlusPadding);
            _encryptionCipher.init(Cipher.ENCRYPT_MODE, _keySpec);
            Main.LogDebug("AESCipher.GetAESKey successful. Secret key: " + _secretKey.toString());
        }
        catch(Exception e){
            Main.LogError("AESCipher.GetAESKey(): " + e.getMessage());
        }
        return _secretKey;
    }
}
