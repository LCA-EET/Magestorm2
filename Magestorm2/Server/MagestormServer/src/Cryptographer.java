import java.math.BigInteger;
import java.security.SecureRandom;

public class Cryptographer {
    private BigInteger _alpha, _p;
    private BigInteger _kPub, _kPr;

    public static BigInteger ModChecksum = BigInteger.valueOf(32767);
    private final SymmetricCipher _cipher;
    private final SecureRandom _secureRandom;

    public Cryptographer(int cipherCode){
        _secureRandom = new SecureRandom();
        switch(cipherCode){
            case ControlCodes.CipherCode_AES:
                _cipher = new AESCipher(this);
                break;
            default:
                _cipher = new SymmetricCipher(this);
                break;
        }
    }
    public SymmetricCipher GetCipher(){
        return _cipher;
    }
    public byte[] RegenerateDomainParameters(int numBits){
        _p = BigInteger.probablePrime(numBits, _secureRandom);
        _alpha = RandomSelection(_p, _secureRandom);
        Main.LogMessage("DHKE Domain Parameters");
        Main.LogMessage("P: " + _p.toString());
        Main.LogMessage("a: " + _alpha.toString());
        _kPr = RandomSelection(_p, _secureRandom);
        _kPub = _alpha.modPow(_kPr, _p);
        Main.LogMessage("Kpub: "+ _kPub);
        return BuildDPPacket();
    }

    public boolean VerifyChecksum(BigInteger toCheck, BigInteger divisor, short remainder){
        BigInteger calculatedRemainder = toCheck.mod(divisor);
        return calculatedRemainder.shortValue() == remainder;
    }

    public BigInteger GetDHKEPrime(){
        return _p;
    }
    public byte[] GetDHKEPublicKeyBytes(){
        return _kPub.toByteArray();
    }
    public BigInteger GetDHKEPublicKey(){
        return _kPub;
    }
    public BigInteger GetDHKEPrivateKey(){
        return _kPr;
    }
    private byte[] BuildDPPacket(){
        byte[] toReturn;
        byte[] pBytes = _p.toByteArray();
        byte[] pLengthBytes = ByteUtils.ShortToByteArray((short)pBytes.length);
        byte[] aBytes = _alpha.toByteArray();
        byte[] aLengthBytes = ByteUtils.ShortToByteArray((short)aBytes.length);
        toReturn = new byte[1 + 2 + 2 + pBytes.length + aBytes.length];
        toReturn[0] = DHKE_Send.DPReply;
        int index = 1;
        System.arraycopy(pLengthBytes, 0, toReturn, index, 2);
        index += 2;
        System.arraycopy(aLengthBytes, 0, toReturn, index, 2);
        index += 2;
        System.arraycopy(pBytes, 0, toReturn, index, pBytes.length);
        index += pBytes.length;
        System.arraycopy(aBytes, 0, toReturn, index, aBytes.length);
        return toReturn;
    }

    private BigInteger RandomSelection(BigInteger probablePrime, SecureRandom random){
        BigInteger upperBound = probablePrime.subtract(BigInteger.TWO);
        BigInteger result;
        do {
            result = new BigInteger(probablePrime.bitLength(), random);
        } while (result.compareTo(upperBound) > 0 || result.compareTo(BigInteger.TWO) < 0); // reject if result > n or less than 2
        return result;
    }

    public static byte[] ApplyChecksum(byte[] toProcess){
        byte[] toReturn = new byte[toProcess.length + 2];
        System.arraycopy(toProcess, 0, toReturn, 0, toProcess.length);
        BigInteger sum = new BigInteger(toProcess);
        short remainder = sum.mod(Cryptographer.ModChecksum).shortValue();
        Main.LogDebug("Checksum remainder: " + remainder);
        byte[] remainderBytes = ByteUtils.ShortToByteArray(remainder);
        Main.LogDebug("Checksum index: " + (toReturn.length - 2));
        System.arraycopy(remainderBytes, 0, toReturn, toReturn.length - 2, 2);
        return toReturn;
    }


    public static BigInteger First16Bytes(BigInteger value) {
        byte[] byteValue = value.toByteArray();
        byte[] first16 = new byte[16];
        System.arraycopy(byteValue,0,first16,0,16);
        return new BigInteger(first16);
    }

    public byte[] Decrypt(byte[] received){
        return _cipher.Decrypt(received);
    }

    public byte[] Encrypt(byte[] toEncrypt){
        return _cipher.Encrypt(toEncrypt);
    }
    public SecureRandom SecureRandom(){
        return _secureRandom;
    }
}
