using TMPro;
using Unity.VisualScripting;
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
    private RenderTexture _renderTexture;
    public Image BackgroundImage;
    public GameObject ModelContainer;
    private GameObject _model;
    private Camera _modelCamera;
    private RawImage _rawImage;
    public bool Populated
    {
        get { return _populated; }
    }
    private void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(_result == FormResult.Yes)
        {
            _result = FormResult.Pending;
            ComponentRegister.PregamePacketProcessor.SendBytes(Packets.DeleteCharacterPacket(_character.CharacterID));
        }
    }
    public void SetOwningForm(UICharacterSelectForm form, RenderTexture render)
    {
        _owner = form;
        _renderTexture = render;
        _rawImage = GetComponentInChildren<RawImage>();
        _modelCamera = GetComponentInChildren<Camera>();
        _modelCamera.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
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
        switch (buttonType)
        {
            case ButtonType.Delete:
                Game.YesNo(Language.BuildString(49, _character.CharacterName), this);
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
        Destroy(_model);
        _model = ComponentRegister.ModelBuilder.ConstructModel(character.AppearanceBytes, (byte)Team.Neutral, character.CharacterLevel, ModelContainer);
    }
  
}
