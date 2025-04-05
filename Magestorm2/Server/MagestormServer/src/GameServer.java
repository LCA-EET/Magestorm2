import java.net.DatagramPacket;

public class GameServer extends Thread implements PacketProcessor{
    private static Listener _serverListener;

    @Override
    public void run(){
        _serverListener = new Listener(ServerParams.ListeningPort, this);
        new Thread(_serverListener).start();
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {
        byte[] bytesReceived = received.getData();
        for(int i = 0; i < bytesReceived.length; i++){
            System.out.println(bytesReceived[i]);
            Main.LogMessage(String.valueOf(bytesReceived[i]));
        }
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
