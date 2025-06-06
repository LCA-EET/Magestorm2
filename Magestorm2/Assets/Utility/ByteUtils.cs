using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ByteUtils
{
    public static byte BitArrayToByte(BitArray toConvert)
    {
        byte[] temp = new byte[1];
        toConvert.CopyTo(temp, 0);
        return temp[0];
    }
    public static string BytesToUTF8(byte[] decrypted, int index, int length)
    {
        return Encoding.UTF8.GetString(decrypted, index, length);
    }
    public static byte[] UTF8ToBytes(string utf8String)
    {
        return Encoding.UTF8.GetBytes(utf8String);
    }

}

