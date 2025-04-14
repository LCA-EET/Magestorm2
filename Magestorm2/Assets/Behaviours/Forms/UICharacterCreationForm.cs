using TMPro;
using UnityEngine;

public class UICharacterCreationForm : ValidatableForm
{
    public ClassToggleGroup ClassToggleGroup;
    private byte _selectedClass;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void PassedValidation()
    {
        Debug.Log(_selectedClass);
    }
    protected override bool ValidateForm()
    {
        if (base.ValidateForm())
        {
            _selectedClass = ClassToggleGroup.GetChecked();
            if (_selectedClass < 255)
            {
                return true;
            }
            else
            {
                Game.MessageBox(Language.GetBaseString(40));
                return false;
            }
        }
        return false;
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        Debug.Log(buttonType);
        switch (buttonType)
        {
            case ButtonType.Submit:
                if (ValidateForm())
                {
                    PassedValidation();
                }
                break;
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }
}
