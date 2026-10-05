import java.math.BigInteger;

public class DHKEPackets {

    public static byte[] PublicKeyBytes(){
        byte[] kpubBytes = Cryptographer.GetDHKEPublicKeyBytes();
        byte[] toReturn = new byte[1 + 2 + kpubBytes.length];
        //BigInteger kpub = Cryptographer.GetDHKEPublicKey();
        toReturn[0] = DHKE_Send.KeyReply;
        byte[] lengthBytes = ByteUtils.ShortToByteArray((short)kpubBytes.length);
        System.arraycopy(lengthBytes, 0, toReturn, 1, 2);
        System.arraycopy(kpubBytes, 0, toReturn, 3, kpubBytes.length);
        //byte[] resultBytes = ByteUtils.ShortToByteArray(kpub.mod(BigInteger.valueOf(32767)).shortValue());
        //System.arraycopy(resultBytes, 0, toReturn, 3 + kpubBytes.length, 2);
        return toReturn;
    }
    public static byte[] KeyReplyPacket(byte[] publicKeyBytes, BigInteger xorKeyBI){
        byte[] xorKey = xorKeyBI.toByteArray();
        byte[] toReturn = new byte[publicKeyBytes.length + 2 + xorKey.length + 2];
        // copy the kpub bytes into the return array
        System.arraycopy(publicKeyBytes, 0, toReturn, 0, publicKeyBytes.length);
        byte[] xorLengthBytes = ByteUtils.ShortToByteArray((short)xorKey.length);
        System.arraycopy(xorLengthBytes, 0, toReturn, publicKeyBytes.length, 2);
        System.arraycopy(xorKey, 0, toReturn, 2 + publicKeyBytes.length, xorKey.length);
        byte[] sumBytes = new byte[publicKeyBytes.length + 2 + xorKey.length];
        System.arraycopy(toReturn, 0, sumBytes, 0, sumBytes.length);
        byte[] remainderBytes = ByteUtils.ShortToByteArray(new BigInteger(sumBytes).mod(Cryptographer.ModChecksum).shortValue());
        System.arraycopy(remainderBytes, 0, toReturn, 2 + publicKeyBytes.length + xorKey.length, 2);
        return toReturn;
    }

}
