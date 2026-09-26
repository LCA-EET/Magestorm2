using TMPro;
using UnityEngine;
public class MultiOption : ValidatableForm
{
    public int[] OptionStrings;
    public int CaptionStringReference;
    public TMP_Text CaptionTextObject, OptionTextObject;
    private int _selectedOption;
    private void Start()
    {
        AssociateFormToButtons();
        CaptionTextObject.text = Language.GetBaseString(CaptionStringReference);
    }
    public void SetOption(int optionIndex)
    {
        _selectedOption = optionIndex - 1;
        RefreshText();
    }
    public int SelectedOption
    {
        get { return _selectedOption + 1; }
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Misc0:
                IncrementOption(false);
                break;
            case ButtonType.Misc1:
                IncrementOption(true);
                break;
        }
    }
    private void IncrementOption(bool increase)
    {
        _selectedOption += increase ? 1 : -1;
        if(_selectedOption < 0)
        {
            _selectedOption = OptionStrings.Length-1;
        }
        else if (_selectedOption == OptionStrings.Length)
        {
            _selectedOption = 0;
        }
        RefreshText();
    }
    private void RefreshText()
    {
        OptionTextObject.text = Language.GetBaseString(OptionStrings[_selectedOption]);
    }
}
