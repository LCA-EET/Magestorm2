using UnityEngine;
using TMPro;
public class UIPCEditor : ValidatableForm
{
    public StatPanel StatPanel;
    public SkillPanel SkillPanel;
    public SlotSelectView SlotSelectView;
    public BitwiseToggleGroup ClassToggleGroup; 

    private void Awake()
    {
        ComponentRegister.UIPCEditor = this;
        AssociateFormToButtons();
    }
    public void InitForm(PlayerCharacter character)
    {
        if (character == null)
        {
            
        }
        else
        {
            ClassToggleGroup.MarkSelected(character.CharacterClass);
            StatPanel.FillStats(character);
            StatPanel.DisablePanel();
            SkillPanel.FillSkills(character);
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
            byte[] stats = StatPanel.GetStats();
            byte[] appearanceBytes = new byte[5];
            ComponentRegister.PregamePacketProcessor.SendBytes(Pregame_Packets.CreateCharacterPacket(EntriesToValidate[0].GetValue().ToString(),
                ClassToggleGroup.GetSelectedIndex(),
                stats,
                appearanceBytes));
            CloseForm();
        }
    }
    protected override void PassedValidation()
    {
        string proposedName = EntriesToValidate[0].GetValue().ToString();
        if (!ProfanityChecker.ContainsProhibitedLanguage(proposedName))
        {
            ComponentRegister.PregamePacketProcessor.SendBytes(Pregame_Packets.NameCheckPacket(proposedName));
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
                    Game.MessageBox(_validationFailureMessages);
                }
                break;
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }
}
