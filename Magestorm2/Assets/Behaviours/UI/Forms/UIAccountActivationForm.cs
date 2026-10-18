using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
public class UIAccountActivationForm : ValidatableForm
{
    public TMP_Text EmailString;
    private int _accountID;
    public void InitForm(int accountID, string email)
    {
        _accountID = accountID;
        Debug.Log(accountID);
        EmailString.text = Language.BuildString(420, email);
    }

    protected override void PassedValidation()
    {
        int otp;
        if(int.TryParse(GetStringEntry(0), out otp)){
            Game.SendPregameBytes(Pregame_Packets.OTPReplyPacket(_accountID, otp));
            CloseForm();
        }
        else
        {
            Game.MessageBoxReference(423);
        }
            
    }
}
