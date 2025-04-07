using UnityEngine;

public class ValidatableForm : InstantiatableForm
{
    public ValidateableObject[] EntriesToValidate;
    public FormButton[] FormButtons;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void AssociateFormToButtons()
    {
        foreach (FormButton button in FormButtons)
        {
            button.SetForm(this);
        }
    }
    protected virtual void PassedValidation()
    {

    }
    protected virtual bool ValidateForm()
    {
        bool passValidation = true;
        foreach (ValidateableObject toValidate in EntriesToValidate)
        {
            if (!toValidate.Validate())
            {
                toValidate.MarkInvalid(true);
                passValidation = false;
            }
        }
        if (!passValidation)
        {
            ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(19), gameObject, transform.parent);
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
