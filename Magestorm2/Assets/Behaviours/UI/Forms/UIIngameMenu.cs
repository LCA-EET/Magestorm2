using TMPro;
using UnityEngine;

public class UIIngameMenu : ValidatableForm
{
    public TMP_Text MatchID;
    public TMP_Text MatchType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        MatchID.text = Language.BuildString(97, MatchParams.IDinMatch);
        MatchType.text = Language.BuildString(111, SharedFunctions.MatchTypeString((MatchTypes)MatchParams.MatchType));
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
                ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.QuitGamePacket());
                Game.Quit();
                break;
        }
    }
}
