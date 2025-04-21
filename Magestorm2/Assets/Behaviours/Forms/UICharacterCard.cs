using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterCard : ValidatableForm
{
    public GameObject NewPanel;
    public GameObject ExistingPanel;
    public TMP_Text CharacterName;
    public TMP_Text CharacterClass;
    public TMP_Text CharacterLevel;
    private bool _populated;
    private PlayerCharacter _character;
    private UICharacterSelectForm _owner;
    private bool _selected;
    public Image BackgroundImage;
    public bool Populated
    {
        get { return _populated; }
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
    public void SetOwningForm(UICharacterSelectForm form)
    {
        _owner = form;
    }
    public void MarkSelected(bool selected)
    {
        _selected = selected;
        PlayerAccount.SelectedCharacter = selected ? _character : null;
        BackgroundImage.color = Colors.ApplyCardSelectionColor(_selected);
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        Debug.Log("CC Button Pressed: " + buttonType);
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
            case ButtonType.Select:
                if (_populated)
                {
                    _owner.CardSelected(this);
                }
                else
                {
                    ComponentRegister.UIPrefabManager.InstantiateCharacterCreator();
                }
                break;
        }
    }
    public void ActivatePanel(bool newCharacter)
    {
        NewPanel.SetActive(newCharacter);
        ExistingPanel.SetActive(!newCharacter);
        _populated = !newCharacter;
    }
    public void Populate(PlayerCharacter character)
    {
        ActivatePanel(false);
        _character = character;
        CharacterName.text = character.CharacterName;
        CharacterClass.text = character.CharacterClassString;
        CharacterLevel.text = character.CharacterLevel.ToString();
    }
  
}
