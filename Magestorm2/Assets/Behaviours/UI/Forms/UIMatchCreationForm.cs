using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIMatchCreationForm : ValidatableForm
{
    public LevelEntry[] LevelEntries;
    public SelectionGroup LevelSelection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        PopulateLevelData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void PopulateLevelData()
    {
        List<Level> levelList = LevelData.GetLevelList();
        int levelCount = levelList.Count;
        int index = 0;
        while (index < levelCount)
        {
            LevelEntries[index].AssociateLevel(levelList[index]);
            index++;
        }
        while (index < LevelEntries.Length)
        {
            LevelEntries[index].gameObject.SetActive(false);
            index++;
        }
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        if(buttonType == ButtonType.Submit)
        {
            int selectedIndex = LevelSelection.SelectedIndex;
            if (selectedIndex == -1)
            {
                Game.MessageBox(Language.GetBaseString(59));
            }
            else
            {
                LevelEntry selected = LevelEntries[selectedIndex];
                Game.SendBytes(Packets.CreateMatchPacket(selected.LevelID));
                CloseForm();
            }
        }
        else
        {
            CloseForm();
        }
    }
}
