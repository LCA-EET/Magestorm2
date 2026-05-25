import java.net.DatagramPacket;
import java.net.InetAddress;

public class RemoteClient extends TimedObject{
    private final InetAddress _address;
    private final int _remotePort;
    private String _username;
    private boolean _subscribedToMatches;

    public RemoteClient(DatagramPacket received){
        _subscribedToMatches = false;
        _address = received.getAddress();
        _remotePort = received.getPort();
        SetDurationRemaining(ServerParams.PregameInactivity);
    }

    public InetAddress IPAddress(){
        return _address;
    }

    public void SetNameAndID(String username, int ID){
        _objectID = ID;
        _username = username;
    }

    public void MarkInGame(){
        SetDurationRemaining(0);
    }

    public void SubscribeToMatches(){
        _subscribedToMatches = true;
    }
    public void UnsubscribeFromMatches(){
        _subscribedToMatches = false;
    }
    public boolean IsSubscribedToMatches(){
        return _subscribedToMatches;
    }
    public int GetRemotePort(){
        return _remotePort;
    }
    public String GetUserName(){
        return _username;
    }

    @Override
    public String toString(){
        return "ID: " + _objectID + ", " + _address.toString() + ":" + _remotePort + " " + _username;
    }
}
