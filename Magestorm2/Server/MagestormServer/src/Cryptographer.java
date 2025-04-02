import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import java.nio.ByteBuffer;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.*;

public class Cryptographer {

    private static final String ENCRYPT_ALGO = "AES/GCM/NoPadding";

    private static final int TAG_LENGTH_BIT = 128; // must be one of {128, 120, 112, 104, 96}
    private static final int IV_LENGTH_BYTE = 16;
    private static final int SALT_LENGTH_BYTE = 16;
    private static final Charset UTF_8 = StandardCharsets.UTF_8;

    private static byte[] _key;
    private static byte[] _iv;
    private static ByteBuffer _keyBuffer;
    private static ByteBuffer _ivBuffer;
    public static byte[] Key(){
        return _key;
    }
    public static byte[] IV(){
        return _iv;
    }
    public static ByteBuffer KeyBuffer(){
        return _keyBuffer;
    }
    public static ByteBuffer IVBuffer(){
        return _ivBuffer;
    }
    public static void GenerateKeyAndIV(){
        try{
            SecureRandom random = SecureRandom.getInstanceStrong();
            _iv = new byte[IV_LENGTH_BYTE];
            random.nextBytes(_iv);
            _ivBuffer = ByteBuffer.wrap(_iv);

            KeyGenerator keyGen = KeyGenerator.getInstance("AES");
            keyGen.init(128, random);
            _key = keyGen.generateKey().getEncoded();
            _keyBuffer = ByteBuffer.wrap(_key);
        }
        catch(Exception ex){

        }

    }

    public static byte[] getIV(int numBytes) {
        byte[] nonce = new byte[numBytes];
        new SecureRandom().nextBytes(nonce);
        return nonce;
    }

    // AES secret key
    public static SecretKey getAESKey(int keysize) throws NoSuchAlgorithmException {
        KeyGenerator keyGen = KeyGenerator.getInstance("AES");
        keyGen.init(keysize, SecureRandom.getInstanceStrong());
        return keyGen.generateKey();
    }

    public static String RandomString(int length) {
        int leftLimit = 48; // numeral '0'
        int rightLimit = 122; // letter 'z'
        int targetStringLength = length;
        Random random = new Random();

        return  random.ints(leftLimit, rightLimit + 1)
                .filter(i -> (i <= 57 || i >= 65) && (i <= 90 || i >= 97))
                .limit(targetStringLength)
                .collect(StringBuilder::new, StringBuilder::appendCodePoint, StringBuilder::append)
                .toString();
    }
}
