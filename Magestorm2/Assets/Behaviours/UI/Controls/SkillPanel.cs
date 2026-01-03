using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class SkillPanel : ValidateableObject
{
    public SkillLine[] SkillLines;
    public TMP_Text RemainingText;

    private PeriodicAction _counter;
    private byte _characterLevel;
    private void Start()
    {
        _counter = new PeriodicAction(0.1f, UpdateRemainingPoints, null);
    }
    public void InitControl(byte characterLevel)
    {
        _characterLevel = characterLevel;   
    }
    private void Update()
    {
        if(_counter != null)
        {
            _counter.ProcessAction(Time.deltaTime);
        }
    }
    private void UpdateRemainingPoints()
    {
        int pointsRemaining = (SharedFunctions.GetMaxSkillPointsForLevel(_characterLevel) - GetUsedSkillPoints());
        RemainingText.text = Language.BuildString(286, pointsRemaining);
    }
    public void RefreshClass(PlayerClass playerClass)
    {
        SpellDiscipline[] availableDisciplines = SharedFunctions.DisciplinesByClass(playerClass);
        int index = 0;
        for (int i = 0; i < availableDisciplines.Length; i++)
        {
            SkillLines[i].Init(0, availableDisciplines[i], _characterLevel);
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
        RefreshClass((PlayerClass)pc.CharacterClass);
        for (int i = 0; i < SkillLines.Length; i++) 
        {
            SkillLine toUpdate = SkillLines[i];
            //if (toUpdate.gameObject.activeSelf)
            //{
                byte skillLevel = pc.GetSkillLevel(toUpdate.SpellDiscipline);
                Debug.Log("Updating skill level " + (byte)toUpdate.SpellDiscipline + ": " + skillLevel);
                toUpdate.SetSkillLevel(skillLevel);
            //}
        }
        if(GetUsedSkillPoints() == SharedFunctions.GetMaxSkillPointsForLevel(_characterLevel))
        {
            RemainingText.gameObject.SetActive(false);
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
        Debug.Log("UsedSkillPoints: " + usedSkillPoints);
        return usedSkillPoints;
    }
    public override bool Validate()
    {
        bool toReturn = GetUsedSkillPoints() == SharedFunctions.GetMaxSkillPointsForLevel(_characterLevel);
        _validationFailureMessage = toReturn?"":Language.GetBaseString(291);
        return toReturn;
    }
    public Dictionary<SpellDiscipline, byte> GetDisciplineTable()
    {
        Dictionary<SpellDiscipline, byte> toReturn = new Dictionary<SpellDiscipline, byte>();
        foreach (SkillLine line in SkillLines)
        {
            if (line.gameObject.activeSelf)
            {
                toReturn.Add(line.SpellDiscipline, line.SkillLevel);
            }
        }
        Debug.Log("DT");
        foreach (SpellDiscipline key in toReturn.Keys)
        {
            Debug.Log((byte)key + ": " + toReturn[key].ToString());
        }
        return toReturn;
    }
}
