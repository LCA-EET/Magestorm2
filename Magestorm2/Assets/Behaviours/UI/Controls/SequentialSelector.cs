using Unity.VisualScripting;
using UnityEngine;

public class SequentialSelector : ValidatableForm
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        switch (buttonType)
        {
            case ButtonType.Increase:
                break;
            case ButtonType.Decrease:
                break;
        }
    }
}
