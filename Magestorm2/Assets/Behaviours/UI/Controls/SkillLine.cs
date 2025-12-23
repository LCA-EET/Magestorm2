using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
public class SkillLine : ValidatableForm
{
    public TMP_Text SkillText;
    public TMP_Text LevelText;

    private byte _skillLevel;

    private void Start()
    {
        AssociateFormToButtons();
    }
    public void Init(byte skillLevel, SpellDiscipline skillID)
    {
        _skillLevel = skillLevel;
        SkillText.text = Language.GetBaseString(SharedFunctions.SpellDisciplineStringReference(skillID));
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Increase:
                if(_skillLevel < 3)
                {
                    _skillLevel++;
                }
                break;
            case ButtonType.Decrease:
                if(_skillLevel > 0)
                {
                    _skillLevel--;
                }
                break;
        }
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

    public byte SkillLevel
    {
        get { return _skillLevel; }
    }
}
