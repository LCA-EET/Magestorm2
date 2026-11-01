import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.BitSet;

public class ByteUtils {
    private static final ByteOrder _order = ByteOrder.LITTLE_ENDIAN;

    public static void WriteLongAtIndex_LE(byte[] array, long toWrite, int index){
        array[index] =  (byte) (toWrite & 0xFF);
        array[index + 1] = (byte) ((toWrite >>> 8) & 0xFF);
        array[index + 2] = (byte) ((toWrite >>> 16) & 0xFF);
        array[index + 3] = (byte) ((toWrite >>> 24) & 0xFF);
        array[index + 4] = (byte) ((toWrite >>> 32) & 0xFF);
        array[index + 5] = (byte) ((toWrite >>> 40) & 0xFF);
        array[index + 6] = (byte) ((toWrite >>> 48) & 0xFF);
        array[index + 7] = (byte) ((toWrite >>> 56) & 0xFF);
    }

    public static void WriteIntAtIndex_LE(byte[] array, int toWrite, int index){
        array[index] =  (byte) (toWrite & 0xFF);
        array[index + 1] = (byte) ((toWrite >>> 8) & 0xFF);
        array[index + 2] = (byte) ((toWrite >>> 16) & 0xFF);
        array[index + 3] = (byte) ((toWrite >>> 24) & 0xFF);
    }
    public static void WriteShortAtIndex_LE(byte[] array, short toWrite, int index){
        array[index] =  (byte) (toWrite & 0xFF);
        array[index + 1] = (byte) ((toWrite >>> 8) & 0xFF);
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

    public static byte[] FloatToByteArray(float value){
        ByteBuffer floatBuffer = ByteBuffer.allocate(4);
        floatBuffer.order(_order);
        return  floatBuffer.putFloat(0, value).array();
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
