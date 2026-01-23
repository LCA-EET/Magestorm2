using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class MatchOption
{
    public const byte FastRegen = 0;
    public const byte NoSolidWalls = 1;
    public const byte AntiStack = 2;
    public const byte NoResurrection = 3;
    public const byte NoHealOther = 4;

    private static bool _init = false;
    private static Dictionary<byte, int> _stringRefs;
    public static void Init()
    {
        if (!_init)
        {
            _init = true;
            _stringRefs = new Dictionary<byte, int>();
            _stringRefs.Add(FastRegen, 198);
            _stringRefs.Add(NoSolidWalls, 199);
            _stringRefs.Add(AntiStack, 200);
            _stringRefs.Add(NoResurrection, 201);
            _stringRefs.Add(NoHealOther, 202);
        }
    }

    public static int GetStringReference(byte optionCode)
    {
        return _stringRefs[optionCode];
    }

    public static string BuildOptionsString(byte matchOptions)
    {
        string toReturn = "";
        Debug.Log("Match Option Byte: " + matchOptions);
        BitArray ba = new BitArray(new byte[] { matchOptions });
        
        for (byte i = 0; i < ba.Length; i++)
        {
            Debug.Log("Bit " + i + ": " + ba[i]);
            if (_stringRefs.ContainsKey(i))
            {
                if (ba[i])
                {
                    string optionText = Language.GetBaseString(_stringRefs[i]);
                    toReturn = toReturn == "" ? toReturn = optionText : toReturn += ", " + optionText;
                }
            }
        }
        if(toReturn == "")
        {
            toReturn = Language.GetBaseString(203);
        }
        return toReturn;
    }
}
