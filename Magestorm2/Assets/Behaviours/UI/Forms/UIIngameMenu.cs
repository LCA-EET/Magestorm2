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
        MatchID.text = Language.BuildString(98, MatchParams.MatchID); //
        MatchType.text = Language.BuildString(112, SharedFunctions.MatchTypeString(MatchParams.MatchType)); //
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
                Game.SendInGameBytes(InGame_Packets.LeaveMatchPacket());
                Match.LeaveMatch();
                break;
            case ButtonType.Misc3:
                Game.SendInGameBytes(InGame_Packets.QuitGamePacket());
                Game.Quit();
                break;
            case ButtonType.Misc4:
                ComponentRegister.UIPrefabManager.InstantiateSpellSlotter();
                break;
            case ButtonType.Misc5:
                Debug.Log("Sending leaderboard request.");
                Game.SendInGameBytes(InGame_Packets.LeaderboardPacket());
                break;
        }
    }
}
