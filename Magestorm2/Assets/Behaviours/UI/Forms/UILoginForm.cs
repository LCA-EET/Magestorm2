using System;
using System.Net;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UILoginForm : ValidatableForm
{
    public Toggle RememberToggle;
    protected override void Awake()
    {
        base.Awake ();
        Game.LoggedIn = false;
        CharacterClassManager.Init();
        DisciplineManager.Init();
        SpellManager.Init();
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
                PassedValidation();
            }
            else
            {
                ComponentRegister.UIPrefabManager.InstantiateDHKEPacketProcessor();
            }
        }
    }
    protected override void PassedValidation()
    {
        //Debug.Log("Passed validation.");
        
        string fieldText = ((TextField)EntriesToValidate[2]).GetValue().ToString();
        string[] split = fieldText.Split(":");
        if(split.Length == 2)
        {
            try
            {
                IPHostEntry server = Dns.GetHostEntry(split[0]);
                //Debug.Log(server.AddressList[0].ToString());
                int remoteport;
                if (int.TryParse(split[1], out remoteport))
                {
                    Game.GameServerPort = remoteport;
                    Game.GameServerAddress = server.AddressList[0];
                    PlayerPrefs.SetString("server", fieldText);
                    if (RememberToggle.isOn)
                    {
                        PlayerPrefs.SetString("user", ((TextField)EntriesToValidate[0]).GetValue().ToString());
                        PlayerPrefs.SetString("password", ((TextField)EntriesToValidate[1]).GetValue().ToString());
                    }
                    PlayerPrefs.SetInt("remember", RememberToggle.isOn ? 1 : 0);
                    byte[] keyBytes;
                    if(Cryptographer.GetStoredSecretKey(fieldText, out keyBytes))
                    {
                        Game.PregameKey = keyBytes;
                        if (ComponentRegister.PregamePacketProcessor != null)
                        {
                            Destroy(ComponentRegister.PregamePacketProcessor.gameObject);
                        }
                        ComponentRegister.UIPrefabManager.InstantiatePregamePacketProcessor();
                    }
                    else
                    {
                        if (ComponentRegister.DHKEPacketProcessor == null)
                        {
                            ComponentRegister.UIPrefabManager.InstantiateDHKEPacketProcessor();
                        }
                        ComponentRegister.DHKEPacketProcessor.Init(remoteport, ControlCodes.CipherCode_None, null);
                        
                    }
                }
                else
                {
                    ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(413));
                }
            }
            catch(Exception ex)
            {
                Game.MessageBox(ex.Message);
               
                ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(412));
            }
        }
        else
        {
            ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(412));
        }
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
