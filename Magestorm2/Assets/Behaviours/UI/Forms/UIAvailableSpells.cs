using TMPro;
public class UIAvailableSpells : ValidatableForm, ISpellProcessor
{
    public TMP_Text FormHeader;
    public SpellSelectView SpellSelectView;

    private ISpellProcessor _owner;
    private byte _slotID;

    private void Start()
    {
        AssociateFormToButtons();
    }
    public void InitializeForm(byte slotID, ISpellProcessor owner)
    {
        _slotID = slotID;
        _owner = owner;
        FormHeader.text = Language.BuildString(277, slotID);
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
