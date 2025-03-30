import java.net.InetAddress;
import java.nio.ByteBuffer;

public class ReceivedPacket {
    protected byte[] _data;
    protected InetAddress _sender;
    protected int _senderID;

    public ReceivedPacket(byte[] data, InetAddress sender){
        _data = data;
        _sender = sender;
        _senderID = BytesToInt(1);
    }

    public String BytesToString(int startIndex, int length){
        return new String(ByteArraySegment(_data, startIndex, length), Main.charset);
    }

    public int GetSenderID(){
        return _senderID;
    }

    public int BytesToInt(int startIndex){
        return ByteBuffer.wrap(_data).getInt(startIndex);
    }

    public float BytesToFloat(int startIndex){
        return ByteBuffer.wrap(_data).getFloat(startIndex);
    }

    public static byte[] ByteArraySegment(byte[] input, int startIndex, int length){
        byte[] toReturn = new byte[length];
        int segmentIndex = 0;
        while(segmentIndex < length){
            toReturn[segmentIndex] = input[startIndex + segmentIndex];
            segmentIndex++;
        }
        return toReturn;
    }
}
