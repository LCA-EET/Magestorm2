using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
public class UIPCEditor : ValidatableForm, IToggleGroupOwner
{
    public StatPanel StatPanel;
    public SkillPanel SkillPanel;
    public SlotSelectView SlotSelectView;
    public TextField NameField;
    public BitwiseToggleGroup ClassToggleGroup;

    private byte _characterLevel;
    private PlayerCharacter _character;
    private void Awake()
    {
        ComponentRegister.UIPCEditor = this;
        AssociateFormToButtons();
        ClassToggleGroup.SetOwningForm(this);
    }
    
    public void GroupToggleChange(byte groupID, byte selectedIndex)
    {
        if(groupID == 0 && _character == null)
        {
            SlotSelectView.ClearSelections();
            SkillPanel.RefreshClass((PlayerClass)selectedIndex);
        }
    }
    public void InitForm(PlayerCharacter character)
    {
        if (character == null)
        {
            _characterLevel = 1;
            SkillPanel.InitControl(1);
            SkillPanel.RefreshClass((PlayerClass)ClassToggleGroup.DefaultSelection);
            SlotSelectView.Init(new byte[10], _characterLevel, SkillPanel);
        }
        else
        {
            _character = character;
            _characterLevel = character.CharacterLevel;
            ClassToggleGroup.MarkSelected(character.CharacterClass);
            NameField.SetValue(_character.CharacterName, true);
            StatPanel.FillStats(character);
            StatPanel.DisablePanel();
            SkillPanel.FillSkills(_character);
            SlotSelectView.Init(character.SlottedSpells, _characterLevel, SkillPanel);
        }
        
    }
    public void NameCheckPassed()
    {
        if(StatPanel.StatTotal() < 90)
        {
            Game.MessageBoxReference(290);
        }
        else
        {
            if (_character == null)
            {
                byte[] stats = StatPanel.GetStats();
                byte[] appearanceBytes = new byte[5];
                Game.SendPregameBytes(Pregame_Packets.CreateCharacterPacket(EntriesToValidate[0].GetValue().ToString(),
                    ClassToggleGroup.GetSelectedIndex(),
                    stats,
                    appearanceBytes,
                    SlotSelectView.SlotSelections,
                    SharedFunctions.DisciplineTableToInt(SkillPanel.GetDisciplineTable())));
                
            }
            else
            {
                Game.SendPregameBytes(Pregame_Packets.UpdateSkillsAndSlotsPacket(_character.CharacterID, SharedFunctions.DisciplineTableToInt(SkillPanel.GetDisciplineTable()), SlotSelectView.SlotSelections));
            }
            CloseForm();
        }
    }
    protected override void PassedValidation()
    {
        string proposedName = EntriesToValidate[0].GetValue().ToString();
        if (!ProfanityChecker.ContainsProhibitedLanguage(proposedName))
        {
            if(_character == null)
            {
                Game.SendPregameBytes(Pregame_Packets.NameCheckPacket(proposedName));
            }
            else
            {
                NameCheckPassed();
            }
        }
        else
        {
            Game.MessageBoxReference(30);
        }
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Submit:
                if (ValidateForm(false))
                {
                    PassedValidation();
                }
                else
                {
                    if(_validationFailureMessages == "")
                    {
                        _validationFailureMessages = Language.GetBaseString(20);
                    }
                    Game.MessageBox(_validationFailureMessages);
                }
                break;
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }
}
