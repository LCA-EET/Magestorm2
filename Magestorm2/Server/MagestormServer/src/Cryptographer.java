import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.math.BigInteger;
import java.nio.ByteBuffer;
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
    private static BigInteger _alpha, _p;
    private static BigInteger _kPub, _kPr;
    public static BigInteger _aesKeyBI;
    public static BigInteger ModChecksum = BigInteger.valueOf(32767);
    public static byte[] RegenerateDomainParameters(int numBits){
        SecureRandom random = new SecureRandom();
        _p = BigInteger.probablePrime(numBits, random);
        _alpha = RandomSelection(_p, random);
        Main.LogMessage("DHKE Domain Parameters");
        Main.LogMessage("P: " + _p.toString());
        Main.LogMessage("a: " + _alpha.toString());
        _kPr = RandomSelection(_p, random);
        _kPub = _alpha.modPow(_kPr, _p);
        Main.LogMessage("Kpub: "+ _kPub);
        return BuildDPPacket();
    }
    public static byte[] MD5Digest(BigInteger toHash){
        return MD5Digest(toHash.toByteArray());
    }
    public static boolean VerifyChecksum(BigInteger toCheck, BigInteger divisor, short remainder){
        BigInteger calculatedRemainder = toCheck.mod(divisor);
        return calculatedRemainder.shortValue() == remainder;
    }
    public static byte[] MD5Digest(byte[] toHash){
        try{
            MessageDigest md = MessageDigest.getInstance("MD5");
            return md.digest(toHash);
        }
        catch(Exception ex){}
        return null;
    }
    public static BigInteger GetDHKEPrime(){
        return _p;
    }
    public static byte[] GetDHKEPublicKeyBytes(){
        return _kPub.toByteArray();
    }
    public static BigInteger GetDHKEPublicKey(){
        return _kPub;
    }
    public static BigInteger GetDHKEPrivateKey(){
        return _kPr;
    }
    private static byte[] BuildDPPacket(){
        byte[] toReturn;
        byte[] pBytes = _p.toByteArray();
        byte[] pLengthBytes = ByteUtils.ShortToByteArray((short)pBytes.length);
        byte[] aBytes = _alpha.toByteArray();
        BigInteger sum = _p.add(_alpha);
        Main.LogDebug("BI SUM: " + sum);
        byte[] aLengthBytes = ByteUtils.ShortToByteArray((short)aBytes.length);
        toReturn = new byte[1 + 2 + 2 + pBytes.length + aBytes.length + 2];
        toReturn[0] = DHKE_Send.DPReply;
        int index = 1;
        System.arraycopy(pLengthBytes, 0, toReturn, index, 2);
        index += 2;
        System.arraycopy(aLengthBytes, 0, toReturn, index, 2);
        index += 2;
        System.arraycopy(pBytes, 0, toReturn, index, pBytes.length);
        index += pBytes.length;
        System.arraycopy(aBytes, 0, toReturn, index, aBytes.length);
        short remainder = sum.mod(BigInteger.valueOf(32767)).shortValue();
        Main.LogDebug("Remainder: " + remainder);
        byte[] resultBytes = ByteUtils.ShortToByteArray(remainder);
        System.arraycopy(resultBytes, 0, toReturn, 5 + pBytes.length + aBytes.length, resultBytes.length);
        return toReturn;
    }

    private static BigInteger RandomSelection(BigInteger probablePrime, SecureRandom random){
        BigInteger upperBound = probablePrime.subtract(BigInteger.TWO);
        BigInteger result;
        do {
            result = new BigInteger(probablePrime.bitLength(), random);
        } while (result.compareTo(upperBound) > 0 || result.compareTo(BigInteger.TWO) < 0); // reject if result > n or less than 2
        return result;
    }

    public static int ComputeChecksum(byte[] data){
        int toReturn = 0;
        for(int i = 0; i < data.length; i++){
            //Main.LogMessage(i + ": " + data[i]);
            toReturn += data[i];
        }
        return toReturn;
    }
    public static byte[] AESKey(){
        return _key;
    }
    public static BigInteger AESKeyBI(){
        return _aesKeyBI;
    }
    public static BigInteger First16Bytes(BigInteger value) {
        byte[] byteValue = value.toByteArray();
        byte[] first16 = new byte[16];
        System.arraycopy(byteValue,0,first16,0,16);
        return new BigInteger(first16);
    }
    public static void GenerateKeyAndIV(){
        try{
            SecureRandom random = SecureRandom.getInstanceStrong();
            _iv = random.nextLong();
            _key = getAESKey(128).getEncoded();
            _aesKeyBI = new BigInteger(_key);
            //Main.LogMessage("Key checksum: " + ComputeChecksum(_key) + ", key length: " + _key.length);
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
        //Main.LogMessage(payload[0] + ", length " + payload.length);
        if(GameServer.SymmetricEncryption){
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
        else{
            return payload;
        }
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
        if(GameServer.SymmetricEncryption){
            byte[] ivBytes = new byte[16];
            int payloadLength = received[16];

            if(payloadLength < 0){
                payloadLength += 256;
            }
            byte[] payload = new byte[payloadLength];
            System.arraycopy(received,0,ivBytes,0,ivBytes.length);
            System.arraycopy(received,17, payload, 0, payloadLength);
            try {
                IvParameterSpec iv = new IvParameterSpec(ivBytes);
                _decryptionCipher.init(Cipher.DECRYPT_MODE, _secretKey, iv);
                return _decryptionCipher.doFinal(payload);
            } catch (Exception e) {
                Main.LogError("Cryptographer.Decrypt(): Decryption error: " + e.getMessage());
            }
            return new byte[0];
        }
        return received;
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
