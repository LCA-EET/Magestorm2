import java.lang.reflect.Array;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.ThreadLocalRandom;

public class ByteUtils {
    private static final ByteOrder _order = ByteOrder.LITTLE_ENDIAN;
    private static ByteBuffer _intBuffer;
    private static ByteBuffer _longBuffer;
    private static ByteBuffer _floatBuffer;
    private static ByteBuffer _shortBuffer;

    public static void init(){
        _intBuffer = ByteBuffer.allocate(4);
        _intBuffer.order(_order);
        _longBuffer = ByteBuffer.allocate(8);
        _longBuffer.order(_order);
        _floatBuffer = ByteBuffer.allocate(4);
        _floatBuffer.order(_order);
        _shortBuffer = ByteBuffer.allocate(2);
        _shortBuffer.order(_order);
    }

    public static int ExtractInt(byte[] decrypted, int index){
        return ByteBuffer.wrap(decrypted).order(_order).getInt(index);
    }

    public static float ExtractFloat(byte[] decrypted, int index){
        return ByteBuffer.wrap(decrypted).order(_order).getFloat(index);
    }

    public static byte[] IntToByteArray(int value) {
        return _intBuffer.putInt(0, value).array();
    }

    public static byte[] FloatToByteArray(float value){
        return  _floatBuffer.putFloat(0, value).array();
    }

    public static byte[] ShortToByteArray(short value){return _shortBuffer.putShort(0, value).array();}

    public static byte[] LongToByteArray(long value){
        return _longBuffer.putLong(0, value).array();
    }

    public static byte[] ArrayListToByteArray(ArrayList<byte[]> arrayList, int totalLength, int startIndex){
        byte[] toReturn = new byte[totalLength];
        int index = startIndex;
        for(byte[] bytes : arrayList){
            System.arraycopy(bytes, 0, toReturn, index , bytes.length);
            index += bytes.length;
        }
        return toReturn;
    }
    public static byte[] UTF8toBytes(String[] toEncode, String delimiter, int startIndex){
        String reassembled = "";
        for(int i = startIndex; i < toEncode.length; i++){
            reassembled = reassembled.equals("") ? toEncode[i] : reassembled + delimiter + toEncode[i];
        }
        Main.LogMessage(("Reassembled string: " + reassembled));
        return reassembled.getBytes(StandardCharsets.UTF_8);
    }

    public static String BytesToUTF8(byte[] toConvert){
        return new String(toConvert, StandardCharsets.UTF_8);
    }

    public static String BytesToUTF8(byte[] decrypted, int index, int length)
    {
        byte[] nameBytes = new byte[length];
        System.arraycopy(decrypted, index, nameBytes, 0, length);
        return BytesToUTF8(nameBytes);
    }

    public static ArrayList<Boolean> ByteArrayToBits(byte[] byteArray) {
        ArrayList<Boolean> bitList = new ArrayList<>();
        for (byte b : byteArray) {
            for (int i = 0; i < 8; i++) {
                boolean bit = (b & (1 << i)) != 0;
                bitList.add(bit);
            }
        }
        return bitList;
    }

}
