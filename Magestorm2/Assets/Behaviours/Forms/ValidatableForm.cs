using UnityEngine;

public class ValidatableForm : MonoBehaviour
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
    public virtual void ButtonPressed(ButtonType buttonType)
    {
        if (buttonType == ButtonType.Submit)
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
        }
        if (buttonType == ButtonType.Cancel)
        {
            Debug.Log("Cancel");
        }
    }
}
