import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.nio.ByteBuffer;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.*;

public class Cryptographer {
    private static final String _algorithmPlusPadding = "AES/CBC/PKCS5Padding";
    private static final String _algorithm = "AES";

    private static byte[] _key;
    private static ByteBuffer _keyBuffer;
    private static SecretKeySpec _keySpec;
    private static SecretKey _secretKey;
    private static Cipher _decryptionCipher, _encryptionCipher;

    public static int ComputeChecksum(byte[] data){
        int toReturn = 0;
        for(int i = 0; i < data.length; i++){
            //Main.LogMessage(i + ": " + data[i]);
            toReturn += data[i];
        }
        return toReturn;
    }
    public static byte[] Key(){
        return _key;
    }
    public static ByteBuffer KeyBuffer(){
        return _keyBuffer;
    }
    public static void GenerateKeyAndIV(){
        try{
            SecureRandom random = SecureRandom.getInstanceStrong();
            _key = getAESKey(128).getEncoded();
            _keyBuffer = ByteBuffer.wrap(_key);
            Main.LogMessage("Key checksum: " + ComputeChecksum(_key) + ", key length: " + _key.length);
        }
        catch(Exception ex){
            Main.LogError("Failed to generate key and IV: " + ex.getMessage());
        }
    }

    public static byte[] Decrypt(byte[] received){
        byte[] ivBytes = new byte[16];
        int payloadLength = received[16];

        if(payloadLength < 0){
            payloadLength += 256;
        }
        //Main.LogMessage("Base64 Key: " + Base64.getEncoder().encodeToString(_key));
        //Main.LogMessage("Payload length: " + payloadLength);
        byte[] payload = new byte[payloadLength];
        System.arraycopy(received,0,ivBytes,0,ivBytes.length);
        System.arraycopy(received,17, payload, 0, payload.length);
        //Main.LogMessage("Encrypted Payload: " + Base64.getEncoder().encodeToString(payload));
        //Main.LogMessage("IV64 Key: " + Base64.getEncoder().encodeToString(ivBytes));
        try {
            IvParameterSpec iv = new IvParameterSpec(ivBytes);
            _decryptionCipher.init(Cipher.DECRYPT_MODE, _secretKey, iv);
            return _decryptionCipher.doFinal(payload);
        } catch (Exception e) {
            Main.LogError("Decryption error: " + e.getMessage());
        }
        return new byte[0];
    }

    public static byte[] generateRandomBytes(SecureRandom random, int numBytes) {
        byte[] data = new byte[numBytes];
        random.nextBytes(data);
        return data;
    }

    // AES secret key
    public static SecretKey getAESKey(int keysize) throws NoSuchAlgorithmException {
        KeyGenerator keyGen = KeyGenerator.getInstance(_algorithm);
        keyGen.init(keysize, SecureRandom.getInstanceStrong());
        try{
            _secretKey = keyGen.generateKey();
            _keySpec = new SecretKeySpec(_secretKey.getEncoded(), _algorithm);

            _decryptionCipher = Cipher.getInstance(_algorithmPlusPadding);

            _encryptionCipher = Cipher.getInstance(_algorithmPlusPadding);
            _encryptionCipher.init(Cipher.ENCRYPT_MODE, _keySpec);
        }
        catch(Exception e){
            Main.LogError(e.getMessage());
        }
        return _secretKey;
    }


}
