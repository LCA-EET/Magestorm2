using System.Collections.Generic;
using UnityEngine;

public class UIKeyMapper : ValidatableForm
{
    private Dictionary<InputControl, KeyCode> _controlTable;
    private KeySelector[] _keySelectors;
    void Start()
    {
        _controlTable = InputControls.ControlTableCopy();
        AssociateFormToButtons();
        _keySelectors = GetComponentsInChildren<KeySelector>();
        foreach(KeySelector keySelector in _keySelectors)
        {
            keySelector.SetOwningForm(this);
        }
    }
    public KeyCode GetKeyCode(InputControl control)
    {
        return _controlTable[control];
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Misc0:
                CloseForm();
                break;
            case ButtonType.Misc1:
                ComponentRegister.UIPrefabManager.InstantiateKeyMapper();
                break;
            case ButtonType.Misc2:
                ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.LeaveMatchPacket());
                Match.LeaveMatch();
                break;
        }
    }
}

