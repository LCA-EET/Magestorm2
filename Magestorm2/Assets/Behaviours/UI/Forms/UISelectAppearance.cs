using TMPro;
using UnityEngine;

public class UISelectAppearance : ValidatableForm
{
    private UIModelPreview _preview;
    private int _characterID;
    public TMP_Text NameText;
    private void Awake()
    {
        _preview = GetComponentInChildren<UIModelPreview>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterID = (int)SharedFunctions.Params[0];
        NameText.text = SharedFunctions.Params[1].ToString();
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        byte[] appearanceBytes = _preview.AppearanceBytes();
        Game.SendBytes(Pregame_Packets.UpdateAppearancePacket(appearanceBytes, _characterID));
        PlayerAccount.GetCharacter(_characterID).AppearanceBytes = appearanceBytes;
        PlayerAccount.MarkUpdatesMade();
        CloseForm();
    }
}
