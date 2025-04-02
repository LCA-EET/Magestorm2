import java.net.DatagramPacket;

public class GameServer extends Thread implements PacketProcessor{
    private static Listener _serverListener;

    @Override
    public void run(){
        Cryptographer.GenerateKeyAndIV();
        _serverListener = new Listener(ServerParams.ListeningPort, this);
        new Thread(_serverListener).start();
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {
        byte[] bytesReceived = received.getData();
        if(bytesReceived.length > 0){
            byte opCode = bytesReceived[0];
            switch (opCode){
                case OpCode.Login:
                    break;
                case OpCode.Chat:
                    break;
                case OpCode.CreateAccount:

                    break;
            }
        }
    }
}
