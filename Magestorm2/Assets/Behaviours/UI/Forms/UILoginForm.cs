using System;
using System.Net;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UILoginForm : ValidatableForm
{
    public Toggle RememberToggle;
    public Action ToPerform;
    protected override void Awake()
    {
        base.Awake ();
        Game.LoggedIn = false;
        CharacterClassManager.Init();
        ComponentRegister.UILoginForm = this;
    }
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerPrefs.GetInt("remember") == 1)
        {
            RememberToggle.isOn = true;
            ((TextField)EntriesToValidate[0]).SetValue(PlayerPrefs.GetString("user"), false);
            ((TextField)EntriesToValidate[1]).SetValue(PlayerPrefs.GetString("password"), false);
        }
        ((TextField)EntriesToValidate[2]).SetValue(PlayerPrefs.GetString("server"), false);
        Debug.Log("Time since epoch: " + TimeUtil.CurrentTime());
        AssociateFormToButtons();
        if(Game.GameServerPort > 0)
        {
            
            if (MatchParams.ReturningFromMatch)
            {
                Cursor.lockState = CursorLockMode.None;
                ToPerform = LogIn;
                ObtainKeyAndPerformAction();
            }
            else
            {
                ComponentRegister.UIPrefabManager.InstantiateDHKEPacketProcessor();
            }
        }
    }
    private bool IsValidServer(ref byte[] keyBytes)
    {
        string fieldText = ((TextField)EntriesToValidate[2]).GetValue().ToString().ToLower();
        string[] split = fieldText.Split(":");
        if(split.Length == 2)
        {
            try
            {
                IPHostEntry server = Dns.GetHostEntry(split[0]);
                int remoteport;
                if (int.TryParse(split[1], out remoteport))
                {
                    Game.GameServerPort = remoteport;
                    Game.GameServerAddress = server.AddressList[0];
                    PlayerPrefs.SetString("server", fieldText);
                    if (Game.SpellManagerTable.ContainsKey(fieldText))
                    {
                        Game.ActiveSpellManager = Game.SpellManagerTable[fieldText];
                        Game.ActiveDisciplineManager = Game.DisciplineManagerTable[fieldText];

                    }
                    else
                    {
                        Game.SpellManagerTable.Add(fieldText, new SpellManager());
                        Game.DisciplineManagerTable.Add(fieldText, new DisciplineManager());
                    }
                    Cryptographer.GetStoredSecretKey(fieldText, out keyBytes);
                    return true;
                }
                else
                {
                    Game.MessageBoxReference(413);
                }
            }
            catch (Exception ex)
            {
                Game.MessageBoxReference(412);
            }
        }
        return false;
    }
    private void ObtainKeyDHKE()
    {
        if (ComponentRegister.DHKEPacketProcessor == null)
        {
            ComponentRegister.UIPrefabManager.InstantiateDHKEPacketProcessor();
        }
        ComponentRegister.DHKEPacketProcessor.Init(Game.GameServerPort, ControlCodes.CipherCode_None, null);
    }
  
    private void ObtainKeyAndPerformAction()
    {
        byte[] storedSecret = null;
        if (IsValidServer(ref storedSecret))
        {
            if (storedSecret == null)
            {
                ObtainKeyDHKE();
            }
            else
            {
                Game.PregameKey = storedSecret;
                if (ComponentRegister.PregamePacketProcessor != null)
                {
                    Destroy(ComponentRegister.PregamePacketProcessor.gameObject);
                }
                ComponentRegister.UIPrefabManager.InstantiatePregamePacketProcessor();
                ToPerform();
            }
        }
    }
    private void LogIn()
    {
        if (RememberToggle.isOn)
        {
            PlayerPrefs.SetString("user", ((TextField)EntriesToValidate[0]).GetValue().ToString());
            PlayerPrefs.SetString("password", ((TextField)EntriesToValidate[1]).GetValue().ToString());
        }
        PlayerPrefs.SetInt("remember", RememberToggle.isOn ? 1 : 0);
        ComponentRegister.PregamePacketProcessor.SendBytes(Pregame_Packets.LogInPacket());
    }
    private void CreateAccount()
    {
        ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject);
    }
   
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.LogIn:
                if (ValidateForm())
                {
                    PlayerAccount.StoreCredentials(((TextField)EntriesToValidate[0]).GetValue().ToString(),
                        Cryptographer.SHA256Hash(((TextField)EntriesToValidate[1]).GetValue().ToString()));
                    ToPerform = LogIn;
                    ObtainKeyAndPerformAction();
                }
                break;
            case ButtonType.CreateAccount:
                ToPerform = CreateAccount;
                ObtainKeyAndPerformAction();
                break;
            case ButtonType.Cancel:
                Game.Quit();
                break;
        }
    }
}
