using UnityEngine;
using UnityEngine.UI;

public class ClassToggleGroup : MonoBehaviour
{
    public Toggle[] ClassToggles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool[] Value{
        get
        {
            if (ClassToggles[0].isOn)
            {
                return new bool[] { false, false};
            }
            if (ClassToggles[1].isOn)
            {
                return new bool[] { true, false };
            }
            if (ClassToggles[2].isOn)
            {
                return new bool[] { false, true };
            }
            if (ClassToggles[3].isOn)
            {
                return new bool[] { true, true };
            }
            return new bool[0];
        }
    
    }


    public byte GetChecked()
    {
        for(byte b = 0; b < ClassToggles.Length; b++)
        {
            if (ClassToggles[b].isOn)
            {
                return b;
            }
        }
        return 255;
    }
}
