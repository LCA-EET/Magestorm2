import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class ByteUtils {
    private static final ByteOrder _order = ByteOrder.LITTLE_ENDIAN;
    private static ByteBuffer _intBuffer;
    private static ByteBuffer _longBuffer;
    public static void init(){
        _intBuffer = ByteBuffer.allocate(4);
        _intBuffer.order(_order);
        _longBuffer = ByteBuffer.allocate(8);
        _longBuffer.order(_order);
    }

    public static int ExtractInt(byte[] decrypted, int index){
        return ByteBuffer.wrap(decrypted).order(_order).getInt(index);
    }

    public static byte[] IntToByteArray(int value) {
        return _intBuffer.putInt(0, value).array();
    }

    public static byte[] LongToByteArray(long value){
        return _longBuffer.putLong(0, value).array();
    }
}
