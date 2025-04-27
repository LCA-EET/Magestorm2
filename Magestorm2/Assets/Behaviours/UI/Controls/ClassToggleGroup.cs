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
