using UnityEngine;

public class UICreateAccountForm : ValidatableForm
{
    private int _udpPort; 
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
        Cryptography.EncryptAndSend(Packets.CreateAccountPacket(username, hashedPassword, email), UDPBuilder.GetClient(_udpPort));
        CloseForm();
    }

    public override void SetInstantiator(GameObject instantiator, object[] paramArray)
    {
        base.SetInstantiator(instantiator);
        _udpPort = (int)paramArray[0];
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
