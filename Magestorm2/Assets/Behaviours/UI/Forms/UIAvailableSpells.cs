using TMPro;
using System.Collections.Generic;
public class UIAvailableSpells : ValidatableForm, ISpellProcessor
{
    public TMP_Text FormHeader;
    public SpellSelectView SpellSelectView;

    private ISpellProcessor _owner;
    private byte _slotID, _characterLevel;

    private void Start()
    {
        AssociateFormToButtons();
    }
    public void InitializeForm(byte characterLevel, byte slotID, ISpellProcessor owner, Dictionary<SpellDiscipline, byte> disciplineLevels)
    {
        _slotID = slotID;
        _owner = owner;
        _characterLevel = characterLevel;
        FormHeader.text = Language.BuildString(277, slotID);
        List<SpellData> availableSpells = SpellManager.GetAvailableSpells(characterLevel, disciplineLevels);
        SpellSelectView.AssignKeys
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
        _owner.SelectionMade(new object[] { _slotID, selected.GetData(SpellAttributes.NAME) });
        CloseForm();
    }
}
