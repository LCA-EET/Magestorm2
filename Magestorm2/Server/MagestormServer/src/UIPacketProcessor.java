import java.beans.Encoder;
import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.ArrayList;

public class UIPacketProcessor implements PacketProcessor
{
    private UDPClient _udpClient;
    private int _serverPort = 6000;
    public UIPacketProcessor(){
        _udpClient = new UDPClient(_serverPort, this);
        //_udpSender = new UDPClient(_clientPort, null);
        new Thread(_udpClient).start();
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {
        RemoteClient rc = new RemoteClient(received, _serverPort);
        byte[] receivedBytes = received.getData();
        byte[] decrypted = Cryptographer.Decrypt(receivedBytes);
        byte opCode = decrypted[0];
        Main.LogMessage("OpCode: " + opCode);
        switch (opCode){
            case OpCode_Receive.LogIn:
                HandleLogInPacket(decrypted, rc);
                break;
            case OpCode_Receive.CreateAccount:
                HandleCreateAccountPacket(decrypted, rc);
                break;
        }
    }

    public String[] LogInDetails(byte[] decrypted){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(decrypted, 3);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        String[] toReturn = new String[2];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        return toReturn;
    }

    private void HandleLogInPacket(byte[] decrypted, RemoteClient rc){
        String[] creds = LogInDetails(decrypted);
        String username = creds[0];
        String hashed = creds[1];
        boolean validCreds = Database.ValidateCredentials(username, hashed);
        Main.LogMessage("Valid credentials for user " + username + "? " + validCreds);
        byte[] toSend = validCreds?Packets.LoginSucceededPacket():Packets.LoginFailedPacket();
        _udpClient.Send(toSend, rc);
    }

    public String[] CreateAccountDetails(byte[] decrypted){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(decrypted, 4);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        byte[] emailBytes = toProcess.get(2);
        String[] toReturn = new String[3];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        toReturn[2] = new String(emailBytes, StandardCharsets.UTF_8);
        return toReturn;
    }
    private void HandleCreateAccountPacket(byte[] decrypted, RemoteClient rc){
        String[] creds = CreateAccountDetails(decrypted);
        String username = creds[0];
        String email = creds[2];
        Main.LogMessage("Account creation requested: " + username + ", " + email);
        if(Database.AccountRecordCount(username, email) > 0){
            _udpClient.Send(Packets.AccountExistsPacket(), rc);
            Main.LogMessage("Account " + username + " already exists .");
        }
        else{
            Main.LogMessage("Account " + username + " does not already exist.");
            boolean accountCreated = Database.CreateAccount(username, creds[1], email);
            byte[] toSend = accountCreated?Packets.AccountCreatedPacket():Packets.AccountCreationFailedPacket();
            _udpClient.Send(toSend, rc);
        }
    }
}
