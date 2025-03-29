import java.net.DatagramPacket;

public class GameServer extends Thread implements PacketProcessor{
    private static Listener _serverListener;

    @Override
    public void run(){
        _serverListener = new Listener(6000, this);
        new Thread(_serverListener).start();
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {

    }
}
