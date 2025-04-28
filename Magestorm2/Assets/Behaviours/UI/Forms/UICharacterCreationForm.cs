using TMPro;
using UnityEngine;

public class UICharacterCreationForm : ValidatableForm
{
    public ClassToggleGroup ClassToggleGroup;
    private StatPanel _statPanel;
    private byte _selectedClass;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _statPanel = GetComponentInChildren<StatPanel>();
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void PassedValidation()
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(Packets.CreateCharacterPacket(EntriesToValidate[0].GetValue().ToString(), 
            _selectedClass, 
            _statPanel.GetStats()));
        CloseForm();
    }
    protected override bool ValidateForm()
    {
        //Debug.Log("Character name: " + EntriesToValidate[0].GetValue().ToString());
        if (base.ValidateForm())
        {
            _selectedClass = ClassToggleGroup.GetChecked();
        
            if (_selectedClass < 255)
            {
                return true;
            }
            else
            {
                Game.MessageBox(Language.GetBaseString(40));
                return false;
            }
        }
        return false;
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        Debug.Log(buttonType);
        switch (buttonType)
        {
            case ButtonType.Submit:
                if (ValidateForm())
                {
                    PassedValidation();
                }
                break;
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }
}
