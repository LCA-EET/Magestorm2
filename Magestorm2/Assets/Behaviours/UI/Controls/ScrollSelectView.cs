using System.Collections.Generic;
using UnityEngine;

public class ScrollSelectView : MonoBehaviour, ILabelCollection
{
    public SelectableLabel[] Labels;
    protected byte _selectedOption;

    public virtual void Start()
    {
        //RecordSelection(Labels[0].OptionID);
    }

    public void AssignKeys(Dictionary<byte, int> optionsTable)
    {
        int index = 0;
        foreach (byte key in optionsTable.Keys) {
            if (index < Labels.Length)
            {
                SelectableLabel label = Labels[index];
                label.Register(optionsTable[key], key, this);
                label.MarkSelected(false);
            }
            index++;
        }
        for (int i = index; i < Labels.Length; i++)
        {
            Labels[i].gameObject.SetActive(false);
        }
    }

    public void RecordSelection(byte optionID)
    {
        _selectedOption = optionID;
        foreach (SelectableLabel label in Labels)
        {
            label.MarkSelected(_selectedOption == label.OptionID);
        }
        ProcessSelection();
    }

    protected virtual void ProcessSelection()
    {

    }
}
