using System.Collections.Generic;
using UnityEngine;

public class ScrollSelectView : MonoBehaviour, ILabelCollection
{
    public SelectableLabel[] Labels;
    private int _selectedOption;
    private void Awake()
    {
        
    }
    void Start()
    {
        RecordSelection(Labels[0].OptionID);
    }

    public void AssignKeys(Dictionary<byte, int> optionsTable)
    {
        int index = 0;
        foreach (byte key in optionsTable.Keys) {
            if (index < Labels.Length)
            {
                Labels[index].Register(optionsTable[key], key, this);
            }
            index++;
        }
        for (int i = index; i < Labels.Length; i++)
        {
            Labels[i].gameObject.SetActive(false);
        }
    }

    public void RecordSelection(int optionID)
    {
        _selectedOption = optionID;
        foreach (SelectableLabel label in Labels)
        {
            label.MarkSelected(_selectedOption == label.OptionID);
        }
    }
}
