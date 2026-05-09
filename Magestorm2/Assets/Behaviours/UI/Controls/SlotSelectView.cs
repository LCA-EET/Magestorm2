using System.Collections.Generic;
using UnityEngine;
public class SlotSelectView : ScrollSelectView, ISpellProcessor
{
    private byte _characterLevel;
    private Dictionary<byte,byte> _disciplineTable;
    private byte[] _slottedSpells;
    private const int _noSelectionRef = 276;
    void Awake()
    {
        _slottedSpells = new byte[10];
    }
    public void ClearSelections()
    {
        for(byte slotID = 0; slotID < 10; slotID++)
        {
            ResetLine(slotID);
        }
    }
    private void ResetLine(byte slotID)
    {
        _slottedSpells[slotID] = 0;
        Labels[slotID].UpdateText(_noSelectionRef);
    }
    public void Init(byte[] slottedSpells, byte characterLevel, Dictionary<byte, byte> disciplineTable)
    {
        _disciplineTable = disciplineTable;
        _characterLevel = characterLevel;
        for (byte i = 0; i < slottedSpells.Length; i++)
        {
            byte spellID = slottedSpells[i];
            SpellData slottedSpell = null;
            int referenceID = _noSelectionRef;
            if (SpellManager.GetSpell(spellID, ref slottedSpell))
            {
                referenceID = slottedSpell.SpellNameReference;
            }
            _slottedSpells[i] = spellID;
            Labels[i].Register(referenceID, i, this);
            Labels[i].MarkSelected(false);
        }
    }
    public void RecordSpellSelection(byte slotID, byte spellID)
    {
        _slottedSpells[slotID] = spellID;
    }
    public void SelectionMade(object[] args)
    {
        byte slotID = (byte)args[0];
        int nameRef = (int)args[1];
        byte spellID = (byte)args[2];
        RecordSpellSelection(slotID, spellID);
        Labels[slotID].UpdateText(nameRef);
    }

    protected override void ProcessSelection()
    {
        ComponentRegister.UIPrefabManager.InstantiateAvailableSpellList(_characterLevel, _selectedOption, this, _disciplineTable);
    }

    public byte[] SlotSelections
    {
        get
        {
            return _slottedSpells;
        }
    }

    public void CheckAvailability(byte characterLevel, Dictionary<byte, byte> disciplineLevels)
    {
        Dictionary<byte, SpellData> availableSpells = SpellManager.GetAvailableSpells(characterLevel, disciplineLevels);
        for (byte i = 0; i < _slottedSpells.Length; i++)
        {
            if (!availableSpells.ContainsKey(_slottedSpells[i]))
            {
                ResetLine(i);
            }
        }
    }
}
