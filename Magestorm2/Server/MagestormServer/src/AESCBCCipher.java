import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import java.math.BigInteger;
import java.security.NoSuchAlgorithmException;

public class AESCBCCipher extends SymmetricCipher{
    private SecretKey _secretKey;
    private Cipher _decryptionCipher, _encryptionCipher;
    private final byte[] _ivBytes;
    private final byte[] _receivedIV;
    public AESCBCCipher(Cryptographer owner){
        super(owner);
        _ivBytes = new byte[16];
        _receivedIV = new byte[16];
        try{
            _secretKeyBytes = GetAESKey(128).getEncoded();
            _secretSymmetricKeyBI = new BigInteger(_secretKeyBytes);
        }
        catch(Exception ex){
            Main.LogError("Exception in AESCBCCipher() " + ex.getMessage());
        }
    }

    @Override
    public byte[] Encrypt(byte[] payload) {
        try {
            _iv++;
            ByteUtils.WriteLongAtIndex_LE(_ivBytes, _iv, 0);
            _encryptionCipher.init(Cipher.ENCRYPT_MODE, _secretKey, new IvParameterSpec(_ivBytes));
            byte[] result = new byte[18 + _encryptionCipher.getOutputSize(payload.length)];
            System.arraycopy(_ivBytes, 0, result, 0, 16);
            int ciphertextLength =
                    _encryptionCipher.doFinal(payload, 0, payload.length,
                            result, 18);
            ByteUtils.WriteShortAtIndex_LE(result, (short) ciphertextLength, 16);
            return result;
        }
        catch (Exception e) {
            Main.LogError("AESCBCCipher.Encrypt(): " + e.getMessage());
            return null;
        }
    }

    @Override
    public byte[] Decrypt(byte[] toDecrypt){
        short ciphertextLength = ByteUtils.ExtractShort(toDecrypt, 16);
        System.arraycopy(toDecrypt, 0, _receivedIV, 0, 16);
        try {
            _decryptionCipher.init(Cipher.DECRYPT_MODE, _secretKey, new IvParameterSpec(_receivedIV));
            return _decryptionCipher.doFinal(toDecrypt, 18, ciphertextLength);
        } catch (Exception e) {
            Main.LogError("AESCBCCipher.Decrypt(): Decryption error: " + e.getMessage());
        }
        return new byte[0];
    }
    public SecretKey GetAESKey(int keysize) throws NoSuchAlgorithmException {
        String algorithmPlusPadding = "AES/CBC/PKCS5Padding";
        String algorithm = "AES";
        KeyGenerator keyGen = KeyGenerator.getInstance(algorithm);
        keyGen.init(keysize, _owner.SecureRandom());
        try{
            _secretKey = keyGen.generateKey();
            _decryptionCipher = Cipher.getInstance(algorithmPlusPadding);
            _encryptionCipher = Cipher.getInstance(algorithmPlusPadding);
            Main.LogDebug("AESCBCCipher.GetAESKey successful. Secret key: " + _secretKey.toString());
        }
        catch(Exception e){
            Main.LogError("AESCBCCipher.GetAESKey(): " + e.getMessage());
        }
        return _secretKey;
    }
}
