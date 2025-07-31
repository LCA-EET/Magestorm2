using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BitwiseToggleGroup : ToggleGroup
{
    public Toggle[] Options;
    public byte DefaultSelection = 0;

    public byte GetSelectedIndex()
    {
        for (byte b = 0; b < Options.Length; b++)
        {
            if (Options[b].isOn)
            {
                return b;
            }
        }
        return 255;
    }
    protected override void Awake()
    {
        base.Awake();
        foreach (Toggle toggle in Options)
        {
            toggle.group = this;
        }
        Options[DefaultSelection].Select();
        
    }
    public bool[] GetBits()
    {
        int numBits = (int)Mathf.Ceil(Options.Length / 2);
        byte value = 0;
        for (byte i = 0; i < Options.Length; i++)
        {
            if (Options[i].isOn)
            {
                value = i;
                break;
            }
        }
        BitArray ba = new BitArray(new byte[] { value });
        bool[] toReturn = new bool[numBits];
        int index = 0;
        while (index < numBits)
        {
            toReturn[index] = ba[index];
            index++;
        }
        return toReturn;
    }
}
