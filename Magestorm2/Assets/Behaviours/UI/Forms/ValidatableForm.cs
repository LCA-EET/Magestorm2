using System.Collections.Generic;
using UnityEngine;

public class ValidatableForm : InstantiatableForm
{
    public ValidateableObject[] EntriesToValidate;
    public FormButton[] FormButtons;
    protected string _validationFailureMessages;
    protected FormResult _result;
    protected Dictionary<ButtonType, FormButton> _buttonTable;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void SetResult(FormResult result)
    {
        _result = result;
    }
    protected void AssociateFormToButtons()
    {
        _buttonTable = new Dictionary<ButtonType, FormButton>();
        foreach (FormButton button in FormButtons)
        {
            button.SetForm(this);
            _buttonTable.Add(button.buttonType, button);
        }
    }
    public void ToggleButtonState(ButtonType buttonType, bool active)
    {
        _buttonTable[buttonType].gameObject.SetActive(active);  
    }
    protected virtual void PassedValidation()
    {

    }
    protected virtual bool ValidateForm()
    {
        return ValidateForm(true);
    }
    protected virtual bool ValidateForm(bool message)
    {
        bool passValidation = true;
        _validationFailureMessages = "";
        foreach (ValidateableObject toValidate in EntriesToValidate)
        {
            if (!toValidate.Validate())
            {
                if(_validationFailureMessages == "")
                {
                    _validationFailureMessages += toValidate.ValidationFailureMessage;
                }
                else
                {
                    _validationFailureMessages += "\n" + toValidate.ValidationFailureMessage;
                }
                toValidate.MarkInvalid(true);
                passValidation = false;
            }
        }
        if (!passValidation && message)
        {
            Game.MessageBox(Language.GetBaseString(20)); //
        }
        return passValidation;
    }
    public virtual void ButtonPressed(ButtonType buttonType)
    {
        if (buttonType == ButtonType.Submit)
        {
            if (ValidateForm())
            {
                PassedValidation();
            }
        }
        if (buttonType == ButtonType.Cancel)
        {
            CloseForm();
        }
    }
}
