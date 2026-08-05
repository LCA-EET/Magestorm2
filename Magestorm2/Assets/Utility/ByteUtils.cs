using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
public static class ByteUtils
{

    public static byte BitArrayToByte(BitArray toConvert)
    {
        byte[] temp = new byte[1];
        toConvert.CopyTo(temp, 0);
        return temp[0];
    }
    public static byte[] BitArrayToBytes(BitArray toConvert)
    {
        byte[] toReturn = new byte[(toConvert.Length + 7) / 8];
        toConvert.CopyTo(toReturn, 0);
        return toReturn;
    }
    public static void InsertVector3AtIndex(byte[] array, Vector3 toInsert, int insertionIndex)
    {
        byte[] bytesToInsert = Vector3ToBytes(toInsert);
        Array.Copy(bytesToInsert, 0, array, insertionIndex, 12);
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
    public static void FillArray(ref byte[] toFill, int index, float data)
    {
        BitConverter.GetBytes(data).CopyTo(toFill, index);
    }
    public static void FillArray(ref byte[] toFill, int index, Vector3 data)
    {
        BitConverter.GetBytes(data.x).CopyTo(toFill, index);
        BitConverter.GetBytes(data.y).CopyTo(toFill, index + 4);
        BitConverter.GetBytes(data.z).CopyTo(toFill, index + 8);
    }
    public static void FillArray(ref byte[] toFill, int index, Quaternion data)
    {
        BitConverter.GetBytes(data.x).CopyTo(toFill, index);
        BitConverter.GetBytes(data.y).CopyTo(toFill, index + 4);
        BitConverter.GetBytes(data.z).CopyTo(toFill, index + 8);
        BitConverter.GetBytes(data.w).CopyTo(toFill, index + 12);
    }
    public static byte[] Vector3ToBytes(Vector3 data)
    {
        byte[] toReturn = new byte[12];
        BitConverter.GetBytes(data.x).CopyTo(toReturn, 0);
        BitConverter.GetBytes(data.y).CopyTo(toReturn, 4);
        BitConverter.GetBytes(data.z).CopyTo(toReturn, 8);
        return toReturn;
    }
    public static BitArray ShortToBoolArray(short num)
    {
        byte[] bytes = BitConverter.GetBytes(num);
        BitArray bitArray = new BitArray(bytes);
        return bitArray;
    }
    public static BitArray ByteToBoolArray(byte toConvert)
    {
        return new BitArray(new byte[] { toConvert });
    }
    public static void FillBooleanArray(ref bool[] toFill, int value, int startIndex)
    {
        switch (value)
        {
            // higher index is msb
            case 0:
                toFill[startIndex + 1] = false;
                toFill[startIndex] = false;
                break;
            case 1:
                toFill[startIndex + 1] = false;
                toFill[startIndex] = true;
                break;
            case 2:
                toFill[startIndex + 1] = true;
                toFill[startIndex] = false;
                break;
            case 3:
                toFill[startIndex + 1] = true;
                toFill[startIndex] = true;
                break;
            case 4:
                toFill[startIndex + 2] = true;
                toFill[startIndex + 1] = false;
                toFill[startIndex] = false;
                break;
            case 5:
                toFill[startIndex + 2] = true;
                toFill[startIndex + 1] = false;
                toFill[startIndex] = true;
                break;
            case 6:
                toFill[startIndex + 2] = true;
                toFill[startIndex + 1] = true;
                toFill[startIndex] = false;
                break;
            case 7:
                toFill[startIndex + 2] = true;
                toFill[startIndex + 1] = true;
                toFill[startIndex] = true;
                break;
        }
    }
}

