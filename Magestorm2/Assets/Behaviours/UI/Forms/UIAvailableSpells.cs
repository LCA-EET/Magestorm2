using TMPro;
using System.Collections.Generic;
using UnityEngine;
public class UIAvailableSpells : ValidatableForm, ISpellProcessor
{
    public TMP_Text FormHeader;
    public SpellSelectView SpellSelectView;

    private ISpellProcessor _owner;
    private byte _slotID, _characterLevel;

    private void Start()
    {
        AssociateFormToButtons();
        Debug.Log("UIAvailableSpells form launched");
    }
    public void InitializeForm(byte characterLevel, byte slotID, ISpellProcessor owner, Dictionary<SpellDiscipline, byte> disciplineLevels)
    {
        _slotID = slotID;
        _owner = owner;
        _characterLevel = characterLevel;
        FormHeader.text = Language.BuildString(277, slotID);
        Dictionary<byte, SpellData> availableSpells = SpellManager.GetAvailableSpells(characterLevel, disciplineLevels);
        Dictionary<byte, int> options = new Dictionary<byte, int>();
        foreach(byte spellKey in availableSpells.Keys)
        {
            options.Add(spellKey, availableSpells[spellKey].SpellNameReference);
        }
        SpellSelectView.SetOwningForm(this);
        SpellSelectView.AssignKeys(options);
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }

    public void SelectionMade(object[] args)
    {
        SpellData selected = (SpellData)args[0];
        _owner.SelectionMade(new object[] { _slotID, selected.SpellNameReference, selected.SpellID});
        CloseForm();
    }
}
