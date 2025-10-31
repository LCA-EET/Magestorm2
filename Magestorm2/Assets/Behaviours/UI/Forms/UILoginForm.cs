using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class UILoginForm : ValidatableForm
{
    private int _udpPort;
    private bool _forceLogin = false;
    private void Awake()
    {
        _udpPort = Game.FetchServerInfo();
        ComponentRegister.UILoginForm = this;
    }
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
  
        Debug.Log("Time since epoch: " + TimeUtil.CurrentTime());
        AssociateFormToButtons();
        
        if(_udpPort > 0)
        {
            SharedFunctions.GameServerPort = _udpPort;
            ComponentRegister.UIPrefabManager.InstantiatePregamePacketProcessor();
            if (MatchParams.ReturningFromMatch)
            {
                MatchParams.ReturningFromMatch = false;
                ComponentRegister.UIPrefabManager.InstantiateCharacterSelector();
                if (PlayerAccount.SelectedCharacter != null)
                {
                    Game.SendPregameBytes(Pregame_Packets.SubscribeToMatchesPacket());
                }
                Cursor.lockState = CursorLockMode.None;
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
        if (!_forceLogin)
        {
            _forceLogin = true;
            string hashedPassword = Cryptography.SHA256Hash("Superman123");
            Cryptography.EncryptAndSend(Pregame_Packets.LogInPacket("Superman", hashedPassword), UDPBuilder.GetClient(_udpPort));
        }
    }
    protected override void PassedValidation()
    {
        //Debug.Log("Passed validation.");
        string username = ((TextField)EntriesToValidate[0]).GetValue().ToString();
        string hashedPassword = Cryptography.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString());
        Cryptography.EncryptAndSend(Pregame_Packets.LogInPacket(username, hashedPassword), UDPBuilder.GetClient(_udpPort)); 
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
