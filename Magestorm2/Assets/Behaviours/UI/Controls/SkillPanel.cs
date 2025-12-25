using TMPro;
public class SkillPanel : ValidateableObject
{
    public SkillLine[] SkillLines;
    public TMP_Text RemainingText;

    private byte _characterLevel;
    public void InitControl(byte characterLevel)
    {
        _characterLevel = characterLevel;   
    }
    public void RefreshClass(byte level, PlayerClass playerClass)
    {
        SpellDiscipline[] availableDisciplines = SharedFunctions.DisciplinesByClass(playerClass);
        int index = 0;
        for (int i = 0; i < availableDisciplines.Length; i++)
        {
            SkillLines[i].Init(level, availableDisciplines[i], _characterLevel);
            index++;
        }
        while(index < SkillLines.Length)
        {
            SkillLines[index].gameObject.SetActive(false);
            index++;
        }
    }
    public void FillSkills(PlayerCharacter pc)
    {
        RefreshClass(pc.CharacterLevel, (PlayerClass)pc.CharacterClass);
        for (int i = 0; i < SkillLines.Length; i++) 
        {
            SkillLine toUpdate = SkillLines[i];
            if (toUpdate.gameObject.activeSelf)
            {
                byte skillLevel = pc.GetSkillLevel(toUpdate.SpellDiscipline);
                toUpdate.SetSkillLevel(skillLevel);
            }
        }
        if(GetUsedSkillPoints() == SharedFunctions.GetMaxSkillPointsForLevel(_characterLevel))
        {
            for (int i = 0; i < SkillLines.Length; i++)
            {
                SkillLines[i].DisableButtons();
            }
        }
    }
    private byte GetUsedSkillPoints()
    {
        byte usedSkillPoints = 0;
        for (int i = 0; i< SkillLines.Length; i++)
        {
            SkillLine toCheck = SkillLines[i];
            if (toCheck.gameObject.activeSelf)
            {
                usedSkillPoints += toCheck.SkillLevel;
            }
        }
        return usedSkillPoints;
    }
    public override bool Validate()
    {
        _validationFailureMessage = Language.GetBaseString(291);
        return GetUsedSkillPoints() != SharedFunctions.GetMaxSkillPointsForLevel(_characterLevel);
    }
}
