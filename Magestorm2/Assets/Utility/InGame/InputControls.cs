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
    ToggleMusic = 17,
    ChatScrollUp = 18,
    ChatScrollDown = 19,
    ChatScrollTop=20,
    ChatScrollBottom = 21,
    MiniMapZoomIn=22,
    MiniMapZoomOut=23,
    MiniMapZoomDefault=24,
    InGameMenu = 25
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
            _controls.Add(InputControl.CancelChat, KeyCode.Alpha0); //Escape
            _controls.Add(InputControl.SendMessage, KeyCode.Return);
            _controls.Add(InputControl.PreviousTrack, KeyCode.Minus);
            _controls.Add(InputControl.NextTrack, KeyCode.Plus);
            _controls.Add(InputControl.ToggleMusic, KeyCode.M);
            _controls.Add(InputControl.ChatScrollUp, KeyCode.PageUp);
            _controls.Add(InputControl.ChatScrollDown, KeyCode.PageDown);
            _controls.Add(InputControl.ChatScrollTop, KeyCode.Home);
            _controls.Add(InputControl.ChatScrollBottom, KeyCode.End);
            _controls.Add(InputControl.MiniMapZoomIn, KeyCode.LeftBracket);
            _controls.Add(InputControl.MiniMapZoomOut, KeyCode.RightBracket);
            _controls.Add(InputControl.InGameMenu, KeyCode.Alpha0); //Escape
            _init = true;
        }
    }
    public static bool Action
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.Action]) ) && Match.GameMode; 
        }
    }
    public static bool MiniMapZoomIn
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.MiniMapZoomIn])) && Match.GameMode;
        }
    }

    public static bool MiniMapZoomOut
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.MiniMapZoomOut])) && Match.GameMode;
        }
    }
    public static bool ChatScrollTop
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollTop])) && !Match.MenuMode;
        }
    }
    public static bool ChatScrollBottom
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollBottom])) && !Match.MenuMode;
        }
    }
    public static bool ChatScrollUp
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollUp])) && !Match.MenuMode;
        }
    }
    public static bool ChatScrollDown
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollDown])) && !Match.MenuMode;
        }
    }
    public static bool Run
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.Run]) && !Backward) && Match.GameMode;
        }
    }
    public static bool Jump
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Jump]) && Match.GameMode;
        }
    }
    public static bool ShootPrimary
    {
        get
        {
            return Input.GetKey(_controls[InputControl.ShootPrimary]) && Match.GameMode;
        }
    }
    public static bool Forward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Forward]) && Match.GameMode;
        }
    }
    public static bool Backward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Backward]) && Match.GameMode;
        }
    }
    public static bool StrafeLeft
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeLeft]) && Match.GameMode;
        }
    }
    public static bool StrafeRight
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeRight]) && Match.GameMode;
        }
    }
    public static bool Ascend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Ascend]) && Match.GameMode;
        }
    }
    public static bool Descend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Descend]) && Match.GameMode;
        }
    }
    public static bool HUD
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.HUDToggle]) && Match.GameMode;
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
            return Input.GetKeyDown(_controls[InputControl.ChatMode]) && Match.GameMode;
        }
    }
    public static bool InGameMenu
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.InGameMenu]) && Match.GameMode;
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
            return Input.GetKeyDown(_controls[InputControl.ToggleMusic]) && Match.GameMode;
        }
    }
    public static bool PreviousTrack
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.PreviousTrack]) && Match.GameMode;
        }
    }
    public static bool NextTrack
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.NextTrack]) && Match.GameMode;
        }
    }
    public static bool IsPressed(InputControl key)
    {
        return Input.GetKey(_controls[key]) && Match.GameMode;
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
