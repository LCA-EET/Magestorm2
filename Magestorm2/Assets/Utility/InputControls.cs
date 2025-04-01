using UnityEngine;
using System.Collections.Generic;
public enum InputControl
{
    Forward = 0,
    Backward = 1,
    StrafeLeft = 2,
    StrafeRight = 3,
    Run = 4,
    Jump = 5,
    ShootPrimary = 6,
    ShootSecondary = 7,
    Action = 8,
    Ascend = 9,
    Descend = 10,
    HUDToggle = 11,
    ChatMode = 12,
    CancelChat = 13,
    SendMessage = 14,
    NextTrack = 15,
    PreviousTrack = 16,
    ToggleMusic = 17
}
public static class InputControls
{
    private static Dictionary<InputControl, KeyCode> _controls;

    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _controls = new Dictionary<InputControl, KeyCode>();
            _controls.Add(InputControl.Forward, KeyCode.W);
            _controls.Add(InputControl.Backward, KeyCode.S);
            _controls.Add(InputControl.StrafeLeft, KeyCode.Q);
            _controls.Add(InputControl.StrafeRight, KeyCode.E);
            _controls.Add(InputControl.Run, KeyCode.LeftShift);
            _controls.Add(InputControl.Jump, KeyCode.Space);
            _controls.Add(InputControl.ShootPrimary, KeyCode.LeftControl);
            _controls.Add(InputControl.Action, KeyCode.Return);
            _controls.Add(InputControl.Ascend, KeyCode.PageUp);
            _controls.Add(InputControl.Descend, KeyCode.PageDown);
            _controls.Add(InputControl.HUDToggle, KeyCode.H);
            _controls.Add(InputControl.ChatMode, KeyCode.Quote);
            _controls.Add(InputControl.CancelChat, KeyCode.Escape);
            _controls.Add(InputControl.SendMessage, KeyCode.Return);
            _controls.Add(InputControl.PreviousTrack, KeyCode.Minus);
            _controls.Add(InputControl.NextTrack, KeyCode.Plus);
            _controls.Add(InputControl.ToggleMusic, KeyCode.M);
            _init = true;
        }
    }
    public static bool Run
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.Run]) && !Backward) && !Match.ChatMode;
        }
    }
    public static bool Jump
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Jump]) && !Match.ChatMode;
        }
    }
    public static bool ShootPrimary
    {
        get
        {
            return Input.GetKey(_controls[InputControl.ShootPrimary]) && !Match.ChatMode;
        }
    }
    public static bool Forward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Forward]) && !Match.ChatMode;
        }
    }
    public static bool Backward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Backward]) && !Match.ChatMode;
        }
    }
    public static bool StrafeLeft
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeLeft]) && !Match.ChatMode;
        }
    }
    public static bool StrafeRight
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeRight]) && !Match.ChatMode;
        }
    }
    public static bool Ascend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Ascend]) && !Match.ChatMode;
        }
    }
    public static bool Descend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Descend]) && !Match.ChatMode;
        }
    }
    public static bool HUD
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.HUDToggle]) && !Match.ChatMode;
        }
    }
    public static bool SendMessage
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.SendMessage]) && Match.ChatMode;
        }
    }
    public static bool ChatMode
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.ChatMode]) && !Match.ChatMode;
        }
    }
    public static bool CancelChat
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.CancelChat]) && Match.ChatMode;
        }
    }
    public static bool ToggleMusic
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.ToggleMusic]) && !Match.ChatMode;
        }
    }
    public static bool PreviousTrack
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.PreviousTrack]) && !Match.ChatMode;
        }
    }
    public static bool NextTrack
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.NextTrack]) && !Match.ChatMode;
        }
    }
    public static bool IsPressed(InputControl key)
    {
        return Input.GetKey(_controls[key]) && !Match.ChatMode;
    }
    public static bool IsPressed_IgnoreChatMode(InputControl key)
    {
        return Input.GetKey(_controls[key]);
    }
    public static string KeyToString(InputControl key)
    {
        KeyCode requested = _controls[key];
        return requested.ToString();
    }
    public static bool Initialized
    {
        get
        {
            return _init;
        }
    }
}
