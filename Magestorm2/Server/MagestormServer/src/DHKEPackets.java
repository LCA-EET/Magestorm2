import java.math.BigInteger;
public class DHKEPackets {

    public static byte[] PublicKeyBytes(byte[] publicKeyBytes){
        byte[] toReturn = new byte[1 + 2 + publicKeyBytes.length];
        toReturn[0] = DHKE_Send.KeyReply;
        ByteUtils.WriteShortAtIndex_LE(toReturn, (short)publicKeyBytes.length, 1);
        System.arraycopy(publicKeyBytes, 0, toReturn, 3, publicKeyBytes.length);
        return toReturn;
    }
    public static byte[] KeyReplyPacket(byte[] publicKeyBytes, byte[] xorKey){
        byte[] toReturn = new byte[publicKeyBytes.length + 2 + xorKey.length];
        System.arraycopy(publicKeyBytes, 0, toReturn, 0, publicKeyBytes.length);
        ByteUtils.WriteShortAtIndex_LE(toReturn, (short)xorKey.length, publicKeyBytes.length);
        System.arraycopy(xorKey, 0, toReturn, 2 + publicKeyBytes.length, xorKey.length);
        return toReturn;
    }

    public static byte[] KeyVerificationPacket(byte result){
        byte[] toReturn = new byte[2];
        toReturn[0] = DHKE_Send.CheckPublicKeyReply;
        toReturn[1] = result;
        return toReturn;
    }
}
