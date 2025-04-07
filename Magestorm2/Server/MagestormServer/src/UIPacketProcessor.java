import java.beans.Encoder;
import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;

public class UIPacketProcessor implements PacketProcessor
{
    private UDPClient _udpClient;

    public UIPacketProcessor(){
        _udpClient = new UDPClient(6000, this);
        new Thread(_udpClient).start();
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {
        byte[] receivedBytes = received.getData();
        byte[] decrypted = Cryptographer.Decrypt(receivedBytes);
        byte opCode = decrypted[0];
        Main.LogMessage("OpCode: " + opCode);
        switch (opCode){
            case OpCode_Receive.LogIn:
                String[] creds = LogInDetails(decrypted);
                break;
            case OpCode_Receive.CreateAccount:
                break;
        }
    }

    public String[] LogInDetails(byte[] decrypted){
        byte userNameLength = decrypted[1];
        byte pwHashLength = decrypted[2];
        byte[] userNameBytes = new byte[userNameLength];
        byte[] pwHashBytes = new byte[pwHashLength];
        System.arraycopy(decrypted, 3,userNameBytes, 0, userNameLength);
        System.arraycopy(decrypted, 3 + userNameLength,pwHashBytes,0, pwHashLength);
        String[] toReturn = new String[2];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        return toReturn;
    }
}
