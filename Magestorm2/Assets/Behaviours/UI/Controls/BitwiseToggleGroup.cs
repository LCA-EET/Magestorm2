using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BitwiseToggleGroup : ToggleGroup
{
    private IToggleGroupOwner _owningForm;
    public Toggle[] Options;
    public byte DefaultSelection = 0;
    public byte GroupID;

    private byte _priorSelection;
    private PeriodicAction _action;
    public byte GetSelectedIndex()
    {
        for (byte b = 0; b < Options.Length; b++)
        {
            if (Options[b].isOn)
            {
                return b;
            }
        }
        
        return 255;
    }
    protected override void Awake()
    {
        base.Awake();
        
        foreach (Toggle toggle in Options)
        {
            toggle.group = this;
        }
        Options[DefaultSelection].Select();
        
    }
    public void Update()
    {
        if (_owningForm != null)
        {
            _action.ProcessAction(Time.deltaTime);    
        }
    }
    private void CheckSelection()
    {
        byte selected = GetSelectedIndex();
        if (selected != _priorSelection)
        {
            _priorSelection = selected;
            _owningForm.GroupToggleChange(GroupID, selected);
        }
    }
    public void SetOwningForm(IToggleGroupOwner owningForm)
    {
        _action = new PeriodicAction(0.1f, CheckSelection, null);
        _owningForm = owningForm;
    }
    public void MarkSelected(byte index)
    {
        Options[index].isOn = true;
        foreach (Toggle toggle in Options)
        {
            toggle.enabled = false;
        }
    }
    public bool[] GetBits()
    {
        int numBits = (int)Mathf.Ceil(Options.Length / 2);
        byte value = 0;
        for (byte i = 0; i < Options.Length; i++)
        {
            if (Options[i].isOn)
            {
                value = i;
                break;
            }
        }
        BitArray ba = new BitArray(new byte[] { value });
        bool[] toReturn = new bool[numBits];
        int index = 0;
        while (index < numBits)
        {
            toReturn[index] = ba[index];
            index++;
        }
        return toReturn;
    }
}
