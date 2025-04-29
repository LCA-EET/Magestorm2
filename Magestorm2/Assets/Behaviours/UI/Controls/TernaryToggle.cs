using UnityEngine;
using UnityEngine.UI;

public class TernaryToggle : MonoBehaviour
{
    public Toggle Option0, Option1, Option2;

    public bool[] Value
    {
        get
        {
            if (Option0.isOn)
            {
                return new bool[] { false, false};
            }
            if (Option1.isOn)
            {
                return new bool[] { false, true };
            }
            if (Option2.isOn)
            {
                return new bool[] { true, false };
            }
            return new bool[0];
        }
    }
}
