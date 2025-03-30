import java.net.InetAddress;

public class LoginPacket extends ReceivedPacket {
    private String _userName, _hash;
    public LoginPacket(byte[] data, InetAddress sender) {
        super(data, sender);
        int userLength = _data[5];
        int hashLength = _data[6];
        _userName = BytesToString(7, userLength);
        _hash = BytesToString(7 + userLength, hashLength);
    }
}
