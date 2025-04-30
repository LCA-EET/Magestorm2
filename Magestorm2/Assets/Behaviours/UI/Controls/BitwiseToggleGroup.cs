using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BitwiseToggleGroup : ToggleGroup
{
    public Toggle[] Options;

    protected override void Awake()
    {
        base.Awake();
        foreach (Toggle toggle in Options)
        {
            toggle.group = this;
        }
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
