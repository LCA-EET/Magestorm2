using TMPro;
using UnityEngine;
public class SkillLine : ValidatableForm
{
    public TMP_Text SkillText;
    public TMP_Text LevelText;

    private byte _skillLevel, _characterLevel;
    private byte _skillID;
    private bool _buttonsEnabled;
    
    private void Start()
    {
        AssociateFormToButtons();
    }
    public void Init(byte skillLevel, byte skillID, byte characterLevel)
    {
        _skillLevel = skillLevel;
        Debug.Log("Init: Skill level for skill " + (byte)skillID + " = " + skillLevel);
        _characterLevel = characterLevel;
        _skillID = skillID;
        _buttonsEnabled = true;
        gameObject.SetActive(true);
        SkillText.text = DisciplineManager.GetDiscipline(skillID).DisciplineName;
        RefreshText();
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        if (_buttonsEnabled)
        {
            switch (buttonType)
            {
                case ButtonType.Increase:
                    if (_skillLevel < 3)
                    {
                        if(_skillLevel == 1 && _characterLevel < 8)
                        {
                            Game.MessageBoxReference(292);
                        }
                        else if (_skillLevel == 2 && _characterLevel < 16)
                        {
                            Game.MessageBoxReference(293);
                        }
                        else
                        {
                            _skillLevel++;
                        }
                    }
                    break;
                case ButtonType.Decrease:
                    if (_skillLevel > 0)
                    {
                        _skillLevel--;
                    }
                    break;
            }
            RefreshText();
        }
    }
    public void DisableButtons()
    {
        foreach(FormButton button in FormButtons)
        {
            button.gameObject.SetActive(false);
        }
        _buttonsEnabled = false;
    }
    private void RefreshText()
    {
        int levelTextRef = 279;
        switch (_skillLevel)
        {
            case 1:
                levelTextRef = 280;
                break;
            case 2:
                levelTextRef = 281;
                break;
            case 3:
                levelTextRef = 282;
                break;
        }
        LevelText.text = Language.GetBaseString(levelTextRef);
    }
    public void SetSkillLevel(byte skillLevel)
    {
        Debug.Log("SetSkillLevel: Skill level for skill " + (byte)_skillID + " = " + skillLevel);
        _skillLevel = skillLevel;
        RefreshText();
    }
    public byte SkillLevel
    {
        get { return _skillLevel; }
    }

    public byte SpellDiscipline { get { return _skillID; } }
}
