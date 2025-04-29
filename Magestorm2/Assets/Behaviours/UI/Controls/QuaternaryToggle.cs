using UnityEngine;
using UnityEngine.UI;

public class QuaternaryToggle : MonoBehaviour
{
    public Toggle Option0, Option1, Option2, Option3;
    public bool[] Value
    {
        get
        {
            if (Option0.isOn)
            {
                return new bool[] { false, false };
            }
            if (Option1.isOn)
            {
                return new bool[] { false, true };
            }
            if (Option2.isOn)
            {
                return new bool[] { true, false };
            }
            if (Option3.isOn)
            {
                return new bool[] { true, true };
            }
            return new bool[0];
        }
    }
}
