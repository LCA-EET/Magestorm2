using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterCreationForm : ValidatableForm
{
    public ClassToggleGroup ClassToggleGroup;
    public BinaryToggle SexToggleGroup;
    public BinaryToggle SkinToggleGroup;
    private StatPanel _statPanel;
    private byte _controlByte;
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
            _controlByte, 
            _statPanel.GetStats()));
        CloseForm();
    }
    protected override bool ValidateForm()
    {
        //Debug.Log("Character name: " + EntriesToValidate[0].GetValue().ToString());
        if (base.ValidateForm())
        {
            BitArray controlByte = new BitArray(8, false);
            controlByte[0] = SexToggleGroup.Value;
            controlByte[1] = SkinToggleGroup.Value;
            bool[] classCode = ClassToggleGroup.Value;
            controlByte[2] = classCode[0];
            controlByte[3] = classCode[1];
            byte[] control = new byte[1];
            controlByte.CopyTo(control, 0);
            _controlByte = control[0];
            return true;

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
