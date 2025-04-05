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

    private static long _iv;
    private static byte[] _key;
    private static ByteBuffer _keyBuffer;

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
            _iv = random.nextLong();
            _key = getAESKey(128).getEncoded();

            _keyBuffer = ByteBuffer.wrap(_key);

            Main.LogMessage("Key checksum: " + ComputeChecksum(_key) + ", key length: " + _key.length);
        }
        catch(Exception ex){
            Main.LogError("Failed to generate key and IV: " + ex.getMessage());
        }

    }

    public static byte[] generateRandomBytes(SecureRandom random, int numBytes) {
        byte[] data = new byte[numBytes];
        random.nextBytes(data);
        return data;
    }

    // AES secret key
    public static SecretKey getAESKey(int keysize) throws NoSuchAlgorithmException {
        KeyGenerator keyGen = KeyGenerator.getInstance("AES");
        keyGen.init(keysize, SecureRandom.getInstanceStrong());
        return keyGen.generateKey();
    }


}
