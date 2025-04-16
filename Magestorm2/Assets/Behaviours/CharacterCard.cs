using TMPro;
using UnityEngine;

public class CharacterCard : ValidatableForm
{
    public GameObject NewPanel;
    public GameObject ExistingPanel;
    public TMP_Text CharacterName;
    public TMP_Text CharacterClass;
    public TMP_Text CharacterLevel;
    private bool _wasPopulated;
    private PlayerCharacter _character;
    public bool WasPopulated
    {
        get { return _wasPopulated; }
    }
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
        switch (buttonType)
        {
            case ButtonType.Delete:
                ComponentRegister.PregamePacketProcessor.SendBytes(Packets.DeleteCharacterPacket(_character.CharacterID));
                break;
            case ButtonType.Submit:
                ComponentRegister.UIPrefabManager.InstantiateCharacterCreator();
                break;
            case ButtonType.Edit:
                break;
        }
        if (buttonType == ButtonType.Submit)
        {
            
        }
    }
    public void ActivatePanel(bool newCharacter)
    {
        NewPanel.SetActive(newCharacter);
        ExistingPanel.SetActive(!newCharacter);
    }
    public void Populate(PlayerCharacter character)
    {
        ActivatePanel(false);
        _character = character;
        CharacterName.text = character.CharacterName;
        CharacterClass.text = character.CharacterClassString;
        CharacterLevel.text = character.CharacterLevel.ToString();
        _wasPopulated = true;
    }
  
}
