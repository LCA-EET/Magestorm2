using UnityEngine;

public class UILoginForm : ValidatableForm
{
    private void Awake()
    {
        Game.LoggedIn = false;
        Game.FetchServerInfo();
        CharacterClassManager.Init();
        DisciplineManager.Init();
        SpellManager.Init();
        ComponentRegister.UILoginForm = this;
    }
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
  
        Debug.Log("Time since epoch: " + TimeUtil.CurrentTime());
        AssociateFormToButtons();
        
        if(Game.GameServerPort > 0)
        {
            ComponentRegister.UIPrefabManager.InstantiatePregamePacketProcessor();
            if (MatchParams.ReturningFromMatch)
            {
                MatchParams.ReturningFromMatch = false;
                ComponentRegister.UIPrefabManager.InstantiateCharacterSelector();
                if (PlayerAccount.SelectedCharacter != null)
                {
                    Game.SendPregameBytes(Pregame_Packets.SubscribeToMatchesPacket(false));
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
        #if UNITY_EDITOR
        if (Game.ForceLogin)
        {
            Game.ForceLogin = false;
            string hashedPassword = Cryptography.SHA256Hash("test2");
            Cryptography.EncryptAndSend(Pregame_Packets.LogInPacket("test2", hashedPassword));
        }
        #endif
    }
    protected override void PassedValidation()
    {
        //Debug.Log("Passed validation.");
        string username = ((TextField)EntriesToValidate[0]).GetValue().ToString();
        string hashedPassword = Cryptography.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString());
        Cryptography.EncryptAndSend(Pregame_Packets.LogInPacket(username, hashedPassword)); 
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
                ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject);
                break;
            case ButtonType.Cancel:
                Game.Quit();
                break;
        }
    }
}
