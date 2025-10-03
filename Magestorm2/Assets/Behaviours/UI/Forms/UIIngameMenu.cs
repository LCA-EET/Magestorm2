using UnityEngine;

public class UIIngameMenu : ValidatableForm
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Misc0:
                Game.MenuMode = false;
                Cursor.visible = false;
                CloseForm();
                break;
            case ButtonType.Misc1:
                ComponentRegister.UIPrefabManager.InstantiateKeyMapper();
                break;
            case ButtonType.Misc2:
                ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.LeaveMatchPacket());
                Match.LeaveMatch();
                break;
            case ButtonType.Misc3:
                ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.LeaveMatchPacket());
                Game.Quit();
                break;
        }
    }
}
