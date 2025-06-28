using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;
public class MessageData
{
    private string _message;
    private Color _messageColor;
    public MessageData(string text, string sender) : this(text, sender, Color.white) { }
    public MessageData(string text, string sender, Color messageColor)
    {
        _messageColor = messageColor;
        float secondsElapsed = ComponentRegister.MatchTimer.SecondsElapsed;
        int minutesElapsed = Mathf.FloorToInt(secondsElapsed / 60.0f);
        int seconds = Mathf.FloorToInt(secondsElapsed % 60);
        string minuteString = ApplyPrefix(minutesElapsed);
        string secondString = ApplyPrefix(seconds);
        _message = "[" + minuteString + ":" + secondString + "] " + sender + ": " + text;
        ComponentRegister.MessageRecorder.MessageReceived(this);
    }
    private string ApplyPrefix(int elapsed)
    {
        string toReturn;
        if (elapsed == 0)
        {
            toReturn = "00";
        }
        else if (elapsed < 10)
        {
            toReturn = "0" + elapsed;
        }
        else
        {
            toReturn = elapsed.ToString();
        }
        return toReturn;
    }
    public string Message
    {
        get
        {
            return _message;
        }
    }
    public Color MessageColor
    {
        get
        {
            return _messageColor;
        }
    }
}
