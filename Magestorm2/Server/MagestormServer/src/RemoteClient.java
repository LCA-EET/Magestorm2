import java.net.DatagramPacket;
import java.net.InetAddress;

public class RemoteClient extends TimedObject{
    private final InetAddress _address;
    private int _remotePort;
    private String _username;
    private int _cidJustExited = 0;
    private boolean _subscribedToMatches, _portSwitchPending, _inGame;

    public RemoteClient(DatagramPacket received){
        _subscribedToMatches = false;
        _inGame = false;
        _address = received.getAddress();
        _remotePort = received.getPort();
        SetDurationRemaining(ServerParams.PregameInactivity);
    }

    public InetAddress IPAddress(){
        return _address;
    }

    public int GetDepartedCharacterID(){
        int toReturn = _cidJustExited;
        _cidJustExited = 0;
        return toReturn;
    }
    public void SetDepartingCharacterID(int cid){
        _cidJustExited = cid;
    }
    public void SetNameAndID(String username, int ID){
        _objectID = ID;
        _username = username;
    }

    public void MarkInGame(){
        _inGame = true;
    }
    public void MarkPortSwitchPending(){
        _portSwitchPending = true;
    }

    public void UpdateRemotePort(int newPort){
        _remotePort = newPort;
        _portSwitchPending = false;
        _inGame = false;
    }

    public boolean PortSwitchPending(){
        return _portSwitchPending;
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

    public String ToString(){
        return "ID: " + _objectID + ", " + _address.toString() + ":" + _remotePort + " " + _username;
    }

    @Override
    public boolean ReduceDuration(long msElapsed){
        if(!_inGame){
            return super.ReduceDuration(msElapsed);
        }
        return false;
    }
}
