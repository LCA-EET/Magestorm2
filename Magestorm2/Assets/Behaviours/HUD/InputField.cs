using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputField : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TMP_InputField _tmpTextMessage;
    public GameObject Background;
    public TMP_Text placeHolder;
    public static Team ChatTarget;
    void Start()
    {
        //Background.SetActive(false);
        Colors.TextBackground = Background.GetComponent<Image>().color;
        ChatTarget = Team.Neutral;
        _tmpTextMessage = GetComponent<TMP_InputField>();
        Language.Init();
        placeHolder.text = Language.BuildString(Language.GetBaseString(1), InputControls.KeyToString(InputControl.ChatMode));   //
    }

    // Update is called once per frame
    void Update()
    {
        
        if (InputControls.ChatMode)
        {
            Game.ChatMode = true;
            //Background.SetActive(true);
            _tmpTextMessage.ActivateInputField();
            placeHolder.text = Language.BuildString(Language.GetBaseString(2), InputControls.KeyToString(InputControl.SendMessage), InputControls.KeyToString(InputControl.CancelChat));    //
        }
        if (InputControls.SendMessage)
        {
            string message = _tmpTextMessage.text;
            
            CancelChat();
            if(message.Trim() != "")
            {
                if (!ProfanityChecker.ContainsProhibitedLanguage(message))
                {
                    if (MatchParams.IncludeTeams && !message.StartsWith("/") && ChatTarget != Team.Neutral)
                    {
                        string prepend = "";
                        switch (ChatTarget)
                        {
                            case Team.Chaos:
                                prepend = "/c ";
                                break;
                            case Team.Balance:
                                prepend = "/b ";
                                break;
                            case Team.Order:
                                prepend = "/o ";
                                break;
                        }
                        message = prepend + message;
                    }
                    ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.BroadcastMessagePacket(message));
                }
                else
                {
                    ComponentRegister.Notifier.DisplayNotification(Language.GetBaseString(30)); //
                }
            }
        }    
        if (InputControls.CancelChat)
        { 
            CancelChat();
        }
    }
    private void CancelChat()
    {
        Game.ChatMode = false;
        placeHolder.text = Language.BuildString(Language.GetBaseString(1), InputControls.KeyToString(InputControl.ChatMode)); //
        _tmpTextMessage.text = "";
        _tmpTextMessage.DeactivateInputField();
    }
}
