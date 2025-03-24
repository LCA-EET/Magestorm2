using TMPro;
using UnityEngine;

public class InputField : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TMP_InputField _tmpTextMessage;
    public TMP_Text placeHolder;
    void Start()
    {
        _tmpTextMessage = GetComponent<TMP_InputField>();
        Language.Init();
        InputControls.Init();
        placeHolder.text = Language.BuildString(Language.GetBaseString(0), InputControls.KeyToString(InputControl.ChatMode));
    }

    // Update is called once per frame
    void Update()
    {
        if (InputControls.ChatMode)
        {
            Game.ChatMode = true;
            _tmpTextMessage.ActivateInputField();
            placeHolder.text = Language.BuildString(Language.GetBaseString(1), InputControls.KeyToString(InputControl.SendMessage), InputControls.KeyToString(InputControl.CancelChat));
        }
        if (InputControls.SendMessage)
        {
            ComponentRegister.Notifier.DisplayNotification(_tmpTextMessage.text);
            CancelChat();
        }    
        if (InputControls.CancelChat)
        {
            CancelChat();
        }
    }
    private void CancelChat()
    {
        Game.ChatMode = false;
        placeHolder.text = Language.BuildString(Language.GetBaseString(0), InputControls.KeyToString(InputControl.ChatMode));
        _tmpTextMessage.text = "";
        _tmpTextMessage.DeactivateInputField();
    }
}
