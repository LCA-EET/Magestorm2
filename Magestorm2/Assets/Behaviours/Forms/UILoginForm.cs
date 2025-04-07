using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class UILoginForm : ValidatableForm
{
    private int _udpPort;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        _udpPort = Game.FetchServerInfo();
        if(_udpPort > 0)
        {
            ComponentRegister.UIPrefabManager.InstantiateUIPacketProcessor(_udpPort);
            if (UDPBuilder.StartListening(_udpPort))
            {
                Debug.Log("Listening!");
            }
        }
        else
        {
            Debug.Log("Unable to fetch server info.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void PassedValidation()
    {
        Debug.Log("Passed validation.");
        string username = ((TextField)EntriesToValidate[0]).GetValue().ToString();
        string hashedPassword = Cryptography.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString());
        Cryptography.EncryptAndSend(Packets.LogInPacket(username, hashedPassword), UDPBuilder.GetClient(_udpPort)); 
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        Debug.Log(buttonType);
        switch (buttonType)
        {
            case ButtonType.LogIn:
                if (ValidateForm())
                {
                    PassedValidation();
                }
                else
                {
                    Debug.Log("invalid entries");
                }
                break;
            case ButtonType.CreateAccount:
                ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject, transform.parent);
                break;
        }
    }
}
