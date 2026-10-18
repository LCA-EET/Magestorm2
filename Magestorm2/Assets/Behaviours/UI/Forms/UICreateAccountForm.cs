using UnityEngine;

public class UICreateAccountForm : ValidatableForm
{
    private string _username;
    private string _hashedPassword;
    private string _email;
    private string _hashedPasswordConfirm;
    private string _emailConfirm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }
    protected override bool ValidateForm()
    {
        bool validated = base.ValidateForm();
        if (validated)
        {
            VariableAssignment();
            if (_email.Equals(_emailConfirm))
            {
                if (_hashedPassword.Equals(_hashedPasswordConfirm))
                {
                    return true;
                }
                else
                {
                    Game.MessageBoxReference(419);
                }
            }
            else
            {
                Game.MessageBoxReference(418);
            }
        }
        return false;
    }
    private void VariableAssignment()
    {
        _username = ((TextField)EntriesToValidate[0]).GetValue().ToString();
        _hashedPassword = Cryptographer.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString());
        _email = ((TextField)EntriesToValidate[2]).GetValue().ToString().ToLower();
        _hashedPasswordConfirm = Cryptographer.SHA256Hash(((TextField)EntriesToValidate[3]).GetValue().ToString());
        _emailConfirm = ((TextField)EntriesToValidate[4]).GetValue().ToString().ToLower();
    }
    protected override void PassedValidation()
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(Pregame_Packets.CreateAccountPacket(_username, _hashedPassword, _email));
        CloseForm();
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
