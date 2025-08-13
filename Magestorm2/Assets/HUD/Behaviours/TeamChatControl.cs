using UnityEngine;

public class TeamChatControl : ValidatableForm
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
        _buttonTable[buttonType].CallBack(buttonType);
    }
}
