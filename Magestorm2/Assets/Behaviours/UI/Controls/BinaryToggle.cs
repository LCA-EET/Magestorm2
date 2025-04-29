using UnityEngine;
using UnityEngine.UI;

public class BinaryToggle : MonoBehaviour
{
    public Toggle Option0, Option1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Value
    {
        get
        {
            if (Option1.isOn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
