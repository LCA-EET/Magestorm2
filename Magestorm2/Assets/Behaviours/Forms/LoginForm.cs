using UnityEditor.PackageManager;
using UnityEngine;

public class LoginForm : ValidatableForm
{
    private UDPGameClient _udp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        _udp = Game.FetchServerInfo();
        if(_udp != null)
        {
            _udp.Listen();
            Debug.Log("Listening!");
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
    public override void ButtonPressed(ButtonType buttonType)
    {
        byte[] testData = new byte[] { 4, 8, 16 };
        Cryptography.EncryptAndSend(testData, _udp);
        Debug.Log(buttonType);
        switch (buttonType)
        {
            case ButtonType.CreateAccount:
                ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject, transform.parent);
                break;
        }
    }
}
