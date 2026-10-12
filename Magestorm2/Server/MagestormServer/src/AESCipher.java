import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.math.BigInteger;
import java.nio.ByteBuffer;
import java.security.NoSuchAlgorithmException;

public class AESCipher extends SymmetricCipher{

    private SecretKeySpec _keySpec;
    private SecretKey _secretKey;
    private Cipher _decryptionCipher, _encryptionCipher;
    private final String _algorithmPlusPadding = "AES/CBC/PKCS5Padding";
    private final String _algorithm = "AES";
    private final byte[] _ivBytes;
    private final ByteBuffer _intBuffer;
    public AESCipher(Cryptographer owner){
        super(owner);
        _intBuffer = ByteBuffer.allocate(Integer.BYTES);
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
    public byte[] Encrypt(byte[] payload){
        byte[] toReturn = null;
        _iv++;
        _intBuffer.putInt(0, _iv);
        byte[] ivBytes = _intBuffer.array();
        System.arraycopy(ivBytes, 0, _ivBytes, 0, ivBytes.length);
        IvParameterSpec iv = new IvParameterSpec(_ivBytes);
        try{
            _encryptionCipher.init(Cipher.ENCRYPT_MODE, _secretKey, iv);
            byte[] encryptedPayload = _encryptionCipher.doFinal(payload);
            toReturn = new byte[18 + encryptedPayload.length];
            System.arraycopy(_ivBytes,0, toReturn,0, _ivBytes.length);
            short payloadLength = (short)encryptedPayload.length;
            byte[] lengthBytes = ByteUtils.ShortToByteArray(payloadLength);
            System.arraycopy(lengthBytes, 0, toReturn, 16, 2);
            System.arraycopy(encryptedPayload, 0, toReturn, 18, encryptedPayload.length);
        } catch (Exception e) {
            Main.LogError("AESCipher.Encrypt(): " + e.getMessage());
        }
        return toReturn;
    }

    @Override
    public byte[] Decrypt(byte[] toDecrypt){
        byte[] ivBytes = new byte[16];
        short payloadLength = ByteUtils.ExtractShort(toDecrypt, 16);
        byte[] payload = new byte[payloadLength];
        System.arraycopy(toDecrypt,0,ivBytes,0,ivBytes.length);
        System.arraycopy(toDecrypt,18, payload, 0, payloadLength);
        try {
            IvParameterSpec iv = new IvParameterSpec(ivBytes);
            _decryptionCipher.init(Cipher.DECRYPT_MODE, _secretKey, iv);
            return _decryptionCipher.doFinal(payload);
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
