import java.math.BigInteger;
import java.net.DatagramPacket;
import java.net.InetAddress;

public class RemoteClient extends TimedObject{
    private final InetAddress _address;
    private final int _remotePort;
    private String _username;
    private boolean _subscribedToMatches;
    private BigInteger _matchPassword;

    public RemoteClient(UDPTransmission received){
        _subscribedToMatches = false;
        _address = received.OriginAddress();
        _remotePort = received.OriginPort();
        SetDurationRemaining(ServerParams.PregameInactivity);
    }

    public InetAddress IPAddress(){
        return _address;
    }

    public void SetMatchPassword(BigInteger matchPasswordHash){
        _matchPassword = matchPasswordHash;
    }
    public BigInteger GetMatchPassword(){
        return _matchPassword;
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
        return "Client ID: " + _objectID + ", " + _address.toString() + ":" + _remotePort + " " + _username;
    }
}
