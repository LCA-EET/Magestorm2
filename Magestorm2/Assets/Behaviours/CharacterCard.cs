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
        if (buttonType == ButtonType.Submit)
        {
            ComponentRegister.UIPrefabManager.InstantiateCharacterCreator();
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
        CharacterName.text = character.CharacterName;
        CharacterClass.text = character.CharacterClassString;
        CharacterLevel.text = character.CharacterLevel.ToString();
        _wasPopulated = true;
    }
  
}
