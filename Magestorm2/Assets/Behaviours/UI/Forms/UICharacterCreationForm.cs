using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterCreationForm : ValidatableForm
{
    public BitwiseToggleGroup ClassToggleGroup;
    public UIModelPreview ModelPanel;

    private StatPanel _statPanel;
    private byte _controlByte;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComponentRegister.UICharacterCreationForm = this;
        _statPanel = GetComponentInChildren<StatPanel>();
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void PassedValidation()
    {
        byte[] stats = _statPanel.GetStats();
        byte[] appearanceBytes = ModelPanel.AppearanceBytes();
        ComponentRegister.PregamePacketProcessor.SendBytes(Packets.CreateCharacterPacket(EntriesToValidate[0].GetValue().ToString(),
            ClassToggleGroup.GetSelectedIndex(),
            stats,
            appearanceBytes));
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
