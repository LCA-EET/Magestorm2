import java.net.DatagramPacket;
import java.net.InetAddress;

public class RemoteClient {

    private int _emanatingPort, _receivingPort;
    private InetAddress _address;
    private int _accountID;
    private int _characterID;
    private String _username;
    private long _timeLastReceived = 0;
    private boolean _subscribedToMatches;

    private PlayerCharacter _activeCharacter;

    public RemoteClient(DatagramPacket received, int receivingPort){
        _subscribedToMatches = false;
        _emanatingPort = received.getPort();
        _receivingPort = receivingPort;
        _address = received.getAddress();
        Main.LogMessage("Remote client IP: " + _address.getHostAddress() + ":" + _emanatingPort);
    }

    public InetAddress IPAddress(){
        return _address;
    }

    public int EmanatingPort(){
        return _emanatingPort;
    }

    public int ReceivingPort(){
        return _receivingPort;
    }

    public void SetNameAndID(String username, int ID){
        _accountID = ID;
        _username = username;
        _timeLastReceived = System.currentTimeMillis();
    }

    public int AccountID(){
        return _accountID;
    }

    public boolean TimeOut(){
        return (System.currentTimeMillis() - _timeLastReceived) > GameServer.TimeOut;
    }
    public void SubscribeToMatches(int charID){
        _subscribedToMatches = true;
        _characterID = charID;
        _activeCharacter = CharacterManager.GetCharacter(charID);
    }
    public void UnsubscribeFromMatches(){
        _subscribedToMatches = false;
    }
    public boolean IsSubscribedToMatches(){
        return _subscribedToMatches;
    }
    public PlayerCharacter GetActiveCharacter(){
        return _activeCharacter;
    }
}
