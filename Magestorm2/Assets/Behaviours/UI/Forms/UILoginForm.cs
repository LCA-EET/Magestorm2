using System;
using System.Net;
using System.Numerics;
using UnityEngine;

public class UILoginForm : ValidatableForm
{
    private void Awake()
    {
        Game.LoggedIn = false;
        //Game.FetchServerInfo();
        CharacterClassManager.Init();
        DisciplineManager.Init();
        SpellManager.Init();
        ComponentRegister.UILoginForm = this;
        ((TextField)EntriesToValidate[0]).SetValue("test2", false);
        ((TextField)EntriesToValidate[1]).SetValue("test2", false);
        ((TextField)EntriesToValidate[2]).SetValue(PlayerPrefs.GetString("server", "fosiemods.net:6000"), false);
    }
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Time since epoch: " + TimeUtil.CurrentTime());
        AssociateFormToButtons();
        if(Game.GameServerPort > 0)
        {
            ComponentRegister.UIPrefabManager.InstantiateDHKEPacketProcessor();
            if (MatchParams.ReturningFromMatch)
            {
                Cursor.lockState = CursorLockMode.None;
                MatchParams.ReturningFromMatch = false;
                PassedValidation();
                /*
                ComponentRegister.UIPrefabManager.InstantiateCharacterSelector();
                if (PlayerAccount.SelectedCharacter != null)
                {
                    Game.SendPregameBytes(Pregame_Packets.MatchScoreRequestPacket());
                    Debug.Log("Send score request packet.");
                }
                */
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
        /*
        if (Game.ForceLogin)
        {
            Game.ForceLogin = false;
            PlayerAccount.StoreCredentials("test2", Cryptography.SHA256Hash("test2"));
            Cryptography.EncryptAndSend(Pregame_Packets.LogInPacket());

        }
        */
#endif
    }
    protected override void PassedValidation()
    {
        //Debug.Log("Passed validation.");
        //Cryptography.EncryptAndSend(Pregame_Packets.LogInPacket()); 
        string fieldText = ((TextField)EntriesToValidate[2]).GetValue().ToString();
        string[] split = fieldText.Split(":");
        if(split.Length == 2)
        {
            try
            {
                IPHostEntry server = Dns.GetHostEntry(split[0]);
                Debug.Log(server.AddressList[0].ToString());
                int remoteport;
                if (int.TryParse(split[1], out remoteport))
                {
                    Game.GameServerPort = remoteport;
                    Game.GameServerAddress = server.AddressList[0];
                    PlayerPrefs.SetString("server", fieldText);
                    if(ComponentRegister.DHKEPacketProcessor == null)
                    {
                        ComponentRegister.UIPrefabManager.InstantiateDHKEPacketProcessor();
                    }
                    else
                    {
                        ComponentRegister.DHKEPacketProcessor.Init(remoteport);
                    }
                }
                else
                {
                    ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(413));
                }
            }
            catch(Exception ex)
            {
                ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(412));
            }
        }
        else
        {
            ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(412));
        }
        
        //Game.UDP.Send(Pregame_Packets.DPRequestPacket());
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.LogIn:
                if (ValidateForm())
                {
                    PlayerAccount.StoreCredentials(((TextField)EntriesToValidate[0]).GetValue().ToString(),
                        Cryptography.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString()));
                    
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
