using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class UILoginForm : ValidatableForm
{
    private int _udpPort;

    private void Awake()
    {
        ComponentRegister.UILoginForm = this;
        
        
    }
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
  
        Debug.Log("Time since epoch: " + TimeUtil.CurrentTime());
        AssociateFormToButtons();
        _udpPort = Game.FetchServerInfo();
        if(_udpPort > 0)
        {
            ComponentRegister.UIPrefabManager.InstantiatePregamePacketProcessor(_udpPort);
            UDPBuilder.StartListening(_udpPort);
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
        //Debug.Log("Passed validation.");
        string username = ((TextField)EntriesToValidate[0]).GetValue().ToString();
        string hashedPassword = Cryptography.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString());
        Cryptography.EncryptAndSend(Packets.LogInPacket(username, hashedPassword), UDPBuilder.GetClient(_udpPort)); 
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.LogIn:
                if (ValidateForm())
                {
                    PassedValidation();
                }
                else
                {
                    //Debug.Log("invalid entries");
                }
                break;
            case ButtonType.CreateAccount:
                ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject, _udpPort);
                break;
            case ButtonType.Cancel:
                Game.Quit();
                break;
        }
    }
}
