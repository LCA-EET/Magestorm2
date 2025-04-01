using UnityEngine;

public class LoginForm : ValidatableForm
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        Debug.Log(buttonType);
        switch (buttonType)
        {
            case ButtonType.CreateAccount:
                ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject, transform.parent);
                break;
        }
    }
}
