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
import java.security.MessageDigest;

public class Cryptographer {
    private static final String _algorithmPlusPadding = "AES/CBC/PKCS5Padding";
    private static final String _algorithm = "AES";
    private static final ByteBuffer _longBuffer = ByteBuffer.allocate(Long.BYTES);
    private static byte[] _key;
    private static SecretKeySpec _keySpec;
    private static SecretKey _secretKey;
    private static Cipher _decryptionCipher, _encryptionCipher;
    private static long _iv;

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
    public static void GenerateKeyAndIV(){
        try{
            SecureRandom random = SecureRandom.getInstanceStrong();
            _iv = random.nextLong();
            _key = getAESKey(128).getEncoded();
            Main.LogMessage("Key checksum: " + ComputeChecksum(_key) + ", key length: " + _key.length);
        }
        catch(Exception ex){
            Main.LogError("Cryptographer.GenerateKeyAndIV(): Failed to generate key and IV: " + ex.getMessage());
        }
    }
    private static byte[] LongToBytes(long toConvert){
        _longBuffer.putLong(0, toConvert);
        byte[] longBytes = _longBuffer.array();
        byte[] toReturn = new byte[16];
        System.arraycopy(longBytes,0, toReturn, 0, longBytes.length);
        return toReturn;
    }
    public static byte[] Encrypt(byte[] payload){
        byte[] toReturn = new byte[0];
        _iv++;
        byte[] ivBytes = LongToBytes(_iv);
        IvParameterSpec iv = new IvParameterSpec(ivBytes);
        try{
            _encryptionCipher.init(Cipher.ENCRYPT_MODE, _secretKey, iv);
            byte[] encryptedPayload = _encryptionCipher.doFinal(payload);
            toReturn = new byte[17 + encryptedPayload.length];
            System.arraycopy(ivBytes,0, toReturn,0, ivBytes.length);
            toReturn[16] = (byte)encryptedPayload.length;
            System.arraycopy(encryptedPayload, 0, toReturn, 17, encryptedPayload.length);
        } catch (Exception e) {
            Main.LogError("Cryptographer.Encrypt(): " + e.getMessage());
        }
        return toReturn;
    }
    public static String MD5(String input){
        String toReturn = "";
        try{
            MessageDigest md5 = MessageDigest.getInstance("MD5");
            return Base64.getEncoder().encodeToString(md5.digest(input.getBytes(StandardCharsets.UTF_8)));
        }
        catch(Exception e){}
        return toReturn;
    }

    public static byte[] Decrypt(byte[] received){
        byte[] ivBytes = new byte[16];
        int payloadLength = received[16];

        if(payloadLength < 0){
            payloadLength += 256;
        }
        byte[] payload = new byte[payloadLength];
        System.arraycopy(received,0,ivBytes,0,ivBytes.length);
        System.arraycopy(received,17, payload, 0, payload.length);
        try {
            IvParameterSpec iv = new IvParameterSpec(ivBytes);
            _decryptionCipher.init(Cipher.DECRYPT_MODE, _secretKey, iv);
            return _decryptionCipher.doFinal(payload);
        } catch (Exception e) {
            Main.LogError("Cryptographer.Decrypt(): Decryption error: " + e.getMessage());
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
            Main.LogError("Cryptographer.getAESKey(): " + e.getMessage());
        }
        return _secretKey;
    }

    public static long RandomToken(){
        SecureRandom rand = new SecureRandom();
        return rand.nextLong();
    }
}
