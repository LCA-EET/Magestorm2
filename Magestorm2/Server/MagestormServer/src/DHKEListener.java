import java.math.BigInteger;
import java.net.DatagramPacket;

public class DHKEListener extends UDPProcessor{

    private final BigInteger _kPr, _p, _kPub;
    private final byte[] _dhkeBytes;
    private final byte[] _publicKeyBytes;

    public DHKEListener(int listeningPort) {
        super(listeningPort, ControlCodes.CipherCode_None);
        _dhkeBytes = _cryptographer.RegenerateDomainParameters(ServerParams.DHKEKeyLength);
        _kPub = _cryptographer.GetDHKEPublicKey();
        _kPr = _cryptographer.GetDHKEPrivateKey();
        _p = _cryptographer.GetDHKEPrime();
        _publicKeyBytes = DHKEPackets.PublicKeyBytes(_cryptographer.GetDHKEPublicKeyBytes());
        Main.LogMessage("DHKE listening on port " + listeningPort + ". DHKE packet length: " + _dhkeBytes.length);
    }

    protected boolean ProcessPacket(DatagramPacket received){
        byte[] payload = received.getData();
        _opCode = payload[0];
        _remote = new RemoteClient(received);
        switch(_opCode){
            case DHKE_Receive.DPRequest:
                Main.LogMessage("Send domain parameters to " + _remote.IPAddress());
                EnqueueForSendWithChecksum(_dhkeBytes, _remote);
                return true;
            case DHKE_Receive.ClientPublicKey:
                ProcessClientPublicKey(payload);
                return true;
            case DHKE_Receive.CheckPublickKey:
                ProcessPublicKeyCheck(payload);
                return true;
        }
        return false;
    }
    private void ProcessPublicKeyCheck(byte[] payload){
        short keylength = ByteUtils.ExtractShort(payload, 1);
        byte[] kpubBytes = new byte[keylength];
        System.arraycopy(payload, 3, kpubBytes, 0, keylength);
        BigInteger kpub = new BigInteger(kpubBytes);
        byte checkResult = (byte)(kpub.equals(_kPub) ? 1 : 0);
        EnqueueForSend(DHKEPackets.KeyVerificationPacket(checkResult), _remote);
    }
    private void ProcessClientPublicKey(byte[] payload){
        short keylength = ByteUtils.ExtractShort(payload, 1);
        byte[] kpubBytes = new byte[keylength];
        System.arraycopy(payload, 3, kpubBytes, 0, keylength);
        BigInteger kpub = new BigInteger(kpubBytes);
        //Main.LogDebug("Client kpub: " + kpub);
        if(_cryptographer.VerifyChecksum(kpub, Cryptographer.ModChecksum, ByteUtils.ExtractShort(payload, 3 + keylength))){
            //Main.LogDebug("Checksum match for client kpub.");
            BigInteger sharedSecret = kpub.modPow(_kPr, _p);
            Main.LogDebug("Shared Secret (Hex):" + sharedSecret.toString(16));
            Main.LogDebug("Shared Secret (Decimal): " + sharedSecret);
            BigInteger first16 = Cryptographer.First16Bytes(sharedSecret);
            Main.LogDebug("First16 (Hex):" + first16.toString(16));
            Main.LogDebug("First16 (Decimal):" + first16 + ", " + first16.bitLength());
            //Main.LogDebug("aesKeyBI: " + aesKeyBI + ", " + aesKeyBI.bitLength());
            BigInteger aesKeyBI = GameServer.PregameProcessor().GetSymmetricKeyBI();
            Main.LogDebug("AES Key (Hex): " + aesKeyBI.toString(16));
            Main.LogDebug("AES Key (Decimal): " + aesKeyBI.toString());
            byte[] aesXOR = aesKeyBI.xor(first16).toByteArray();
            BigInteger aesXORBI = new BigInteger(aesXOR);
            Main.LogDebug("AES XOR BI (Hex): " + aesXORBI.toString(16));
            Main.LogDebug("AES XOR BI (Decimal): " + aesXORBI);
            EnqueueForSendWithChecksum(DHKEPackets.KeyReplyPacket(_publicKeyBytes, aesXOR), _remote);
        }
        else{
            Main.LogDebug("Checksum failed for client kpub.");
        }

    }
}
