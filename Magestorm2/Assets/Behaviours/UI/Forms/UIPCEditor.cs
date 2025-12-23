using UnityEngine;
using TMPro;
public class UIPCEditor : ValidatableForm
{
    public TMP_Text HeaderText;
    public StatPanel StatPanel;
    public SkillPanel SkillPanel;
    public SlotSelectView SlotSelectView;

    public void InitForm(PlayerCharacter character)
    {
        HeaderText.text = Language.BuildString(278, character.CharacterName, character.CharacterLevel, character.CharacterClassString);
    }
}
