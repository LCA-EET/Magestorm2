import java.math.BigInteger;

public class DHKEPackets {

    public static byte[] PublicKeyBytes(byte[] publicKeyBytes){
        byte[] toReturn = new byte[1 + 2 + publicKeyBytes.length];
        //BigInteger kpub = Cryptographer.GetDHKEPublicKey();
        toReturn[0] = DHKE_Send.KeyReply;
        byte[] lengthBytes = ByteUtils.ShortToByteArray((short)publicKeyBytes.length);
        System.arraycopy(lengthBytes, 0, toReturn, 1, 2);
        System.arraycopy(publicKeyBytes, 0, toReturn, 3, publicKeyBytes.length);
        //byte[] resultBytes = ByteUtils.ShortToByteArray(kpub.mod(BigInteger.valueOf(32767)).shortValue());
        //System.arraycopy(resultBytes, 0, toReturn, 3 + kpubBytes.length, 2);
        return toReturn;
    }
    public static byte[] KeyReplyPacket(byte[] publicKeyBytes, byte[] xorKey){
        byte[] toReturn = new byte[publicKeyBytes.length + 2 + xorKey.length];
        // copy the kpub bytes into the return array
        System.arraycopy(publicKeyBytes, 0, toReturn, 0, publicKeyBytes.length);
        byte[] xorLengthBytes = ByteUtils.ShortToByteArray((short)xorKey.length);
        System.arraycopy(xorLengthBytes, 0, toReturn, publicKeyBytes.length, 2);
        System.arraycopy(xorKey, 0, toReturn, 2 + publicKeyBytes.length, xorKey.length);
        byte[] sumBytes = new byte[publicKeyBytes.length + 2 + xorKey.length];
        System.arraycopy(toReturn, 0, sumBytes, 0, sumBytes.length);
        return toReturn;
    }

    public static byte[] KeyVerificationPacket(byte result){
        byte[] toReturn = new byte[2];
        toReturn[0] = DHKE_Send.CheckPublicKeyReply;
        toReturn[1] = result;
        return toReturn;
    }
}
