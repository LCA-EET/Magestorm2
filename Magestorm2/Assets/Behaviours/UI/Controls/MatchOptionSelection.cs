using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MatchOptionSelection : MonoBehaviour
{
    public Toggle[] MatchOptions;

    public byte[] GenerateValue()
    {
        BitArray ba = new BitArray(8);
        for (int i = 0; i < MatchOptions.Length; i++)
        {
            ba[i] = MatchOptions[i].isOn;
        }
        byte[] toReturn = new byte[(ba.Length + 7)/ 8];
        ba.CopyTo(toReturn, 0);
        return toReturn;
    }
}
