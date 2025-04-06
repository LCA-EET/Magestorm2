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
    public override void ButtonPressed(ButtonType buttonType)
    {
        byte[] testData = new byte[] { 4, 8, 16 };
        Debug.Log(buttonType);
        switch (buttonType)
        {
            case ButtonType.CreateAccount:
                ComponentRegister.UIPrefabManager.InstantiateCreateAccountForm(gameObject, transform.parent);
                break;
        }
    }
}
