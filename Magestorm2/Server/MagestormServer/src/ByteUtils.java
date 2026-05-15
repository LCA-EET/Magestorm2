import java.lang.reflect.Array;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.BitSet;
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

    public static short ExtractShort(byte[] decrypted, int index){
        if((index + 2) <= decrypted.length){
            return ByteBuffer.wrap(decrypted).order(_order).getShort(index);
        }
        else{
            return -1;
        }
    }
    public static int ExtractInt(byte[] decrypted, int index){
        if((index + 4) <= decrypted.length){
            return ByteBuffer.wrap(decrypted).order(_order).getInt(index);
        }
        else{
            return -1;
        }
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
            reassembled = reassembled.isEmpty() ? toEncode[i] : reassembled + delimiter + toEncode[i];
        }
        return reassembled.getBytes(StandardCharsets.UTF_8);
    }

    public static byte[] UTF8toBytes(String toEncode){
        return toEncode.getBytes(StandardCharsets.UTF_8);
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

    public static void FillBooleanArray(boolean[] toFill, int value, int startIndex){
        switch(value){
            // higher index is the msb
            case 0:
                toFill[startIndex + 1]  = false;
                toFill[startIndex]      = false;
                break;
            case 1:
                toFill[startIndex + 1]  = false;
                toFill[startIndex]      = true;
                break;
            case 2:
                toFill[startIndex + 1]  = true;
                toFill[startIndex]      = false;
                break;
            case 3:
                toFill[startIndex + 1]  = true;
                toFill[startIndex]      = true;
                break;
            case 4:
                toFill[startIndex + 2]  = true;
                toFill[startIndex + 1]  = false;
                toFill[startIndex]      = false;
                break;
            case 5:
                toFill[startIndex + 2]  = true;
                toFill[startIndex + 1]  = false;
                toFill[startIndex]      = true;
                break;
            case 6:
                toFill[startIndex + 2]  = true;
                toFill[startIndex + 1]  = true;
                toFill[startIndex]      = false;
                break;
            case 7:
                toFill[startIndex + 2]  = true;
                toFill[startIndex + 1]  = true;
                toFill[startIndex]      = true;
                break;
        }
    }
    public static boolean[] ShortToBoolArray(short toConvert)
    {
        int num = toConvert;
        boolean[] binary = new boolean[16];
        int id = 0;

        while (num != 0) {
            binary[id++] = num % 2 != 0;
            num = num / 2;
        }
        return binary;
    }
    public static boolean[] IntegerToBoolArray(int num)
    {
        boolean[] binary = new boolean[32];
        int id = 0;

        while (num != 0) {
            binary[id++] = num % 2 != 0;
            num = num / 2;
        }
        return binary;
    }
    public static boolean[] LongToBoolArray(long num)
    {
        boolean[] binary = new boolean[64];
        int id = 0;

        while (num != 0) {
            binary[id++] = num % 2 != 0;
            num = num / 2;
        }
        return binary;
    }
    static boolean[] BytesToBooleanArray(byte[] bytes) {
        BitSet bits = BitSet.valueOf(bytes);
        boolean[] bools = new boolean[bytes.length * 8];
        for (int i = bits.nextSetBit(0); i != -1; i = bits.nextSetBit(i+1)) {
            bools[i] = true;
        }
        return bools;
    }
    public static int BitsToInt(boolean[] bits) {
        int result = 0;
        for(int i = 0; i < bits.length; i++){
            double toAdd = bits[i] ? Math.pow(2,i) : 0;
            //Main.LogMessage("Bit " + i + " is " + bits[i] + ", adding " + toAdd);
            result += toAdd;
        }
        return result;
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
