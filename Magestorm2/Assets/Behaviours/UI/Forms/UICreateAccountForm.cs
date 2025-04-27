using UnityEngine;

public class UICreateAccountForm : ValidatableForm
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
    protected override void PassedValidation()
    {
        string username = ((TextField)EntriesToValidate[0]).GetValue().ToString();
        string hashedPassword = Cryptography.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString());
        string email = ((TextField)EntriesToValidate[2]).GetValue().ToString();
        ComponentRegister.PregamePacketProcessor.SendBytes(Packets.CreateAccountPacket(username, hashedPassword, email));
        CloseForm();
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
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
