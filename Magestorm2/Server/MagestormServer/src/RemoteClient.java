import java.net.DatagramPacket;
import java.net.InetAddress;

public class RemoteClient {

    private final InetAddress _address;
    private int _accountID;
    private int _remotePort;
    private String _username;
    private long _timeLastReceived = 0;
    private boolean _subscribedToMatches, _portSwitchPending;

    public RemoteClient(DatagramPacket received){
        _subscribedToMatches = false;
        _address = received.getAddress();
        _remotePort = received.getPort();
        MarkPacketReceived();
        //Main.LogMessage("Remote client IP: " + _address.getHostAddress() + ":" + _emanatingPort);
    }

    public InetAddress IPAddress(){
        return _address;
    }

    public void SetNameAndID(String username, int ID){
        _accountID = ID;
        _username = username;
    }

    public void MarkPortSwitchPending(){
        _portSwitchPending = true;
    }

    public void UpdateRemotePort(int newPort){
        _remotePort = newPort;
        _portSwitchPending = false;
    }

    public boolean PortSwitchPending(){
        return _portSwitchPending;
    }

    public int AccountID(){
        return _accountID;
    }

    public boolean TimeOut(){
        return (System.currentTimeMillis() - _timeLastReceived) > GameServer.TimeOut;
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
    public void MarkPacketReceived()
    {
        _timeLastReceived = System.currentTimeMillis();
    }

}
