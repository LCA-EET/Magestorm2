using System.Text;
using TMPro;
using UnityEngine;

public class InputField : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TMP_InputField _tmpTextMessage;
    public TMP_Text placeHolder;
    public static Team ChatTarget;
    void Start()
    {
        ChatTarget = Team.Neutral;
        _tmpTextMessage = GetComponent<TMP_InputField>();
        Language.Init();
        placeHolder.text = Language.BuildString(Language.GetBaseString(0), InputControls.KeyToString(InputControl.ChatMode));
    }

    // Update is called once per frame
    void Update()
    {
        if (InputControls.ChatMode)
        {
            Match.ChatMode = true;
            _tmpTextMessage.ActivateInputField();
            placeHolder.text = Language.BuildString(Language.GetBaseString(1), InputControls.KeyToString(InputControl.SendMessage), InputControls.KeyToString(InputControl.CancelChat));
        }
        if (InputControls.SendMessage)
        {
            string message = _tmpTextMessage.text;
            CancelChat();
            if (!ProfanityChecker.ContainsProhibitedLanguage(message))
            {
                if (message.StartsWith("/"))
                {

                }
                else
                {
                    ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.BroadcastMessagePacket(message));
                }
                    
            }
            else
            {
                ComponentRegister.Notifier.DisplayNotification(Language.GetBaseString(29));
            }
            
        }    
        if (InputControls.CancelChat)
        {
            CancelChat();
        }
    }
    private void CancelChat()
    {
        Match.ChatMode = false;
        placeHolder.text = Language.BuildString(Language.GetBaseString(0), InputControls.KeyToString(InputControl.ChatMode));
        _tmpTextMessage.text = "";
        _tmpTextMessage.DeactivateInputField();
    }
}
