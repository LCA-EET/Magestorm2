using System;
using System.Collections;
using System.Text;
using UnityEngine;
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
    public static Vector3 BytesToVector3(byte[] decrypted, int index)
    {
        return new Vector3(BitConverter.ToSingle(decrypted, index), 
            BitConverter.ToSingle(decrypted, index + 4),
            BitConverter.ToSingle(decrypted, index + 8));

    }
   

}

