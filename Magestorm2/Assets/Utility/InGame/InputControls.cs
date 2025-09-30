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
    InGameMenu = 25,
    Tap = 26,
    Slot1 = 28,
    Slot2 = 29,
    Slot3 = 30,
    Slot4 = 31,
    Slot5 = 32,
    Slot6 = 33,
    Slot7 = 34,
    Slot8 = 35,
    Slot9 = 36,
    Slot10 = 37
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
            _controls.Add(InputControl.ShootSecondary, KeyCode.LeftAlt);
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
            _controls.Add(InputControl.MiniMapZoomDefault, KeyCode.Backslash);
            _controls.Add(InputControl.InGameMenu, KeyCode.Alpha0); //Escape
            _controls.Add(InputControl.Slot1, KeyCode.Alpha1);
            _controls.Add(InputControl.Slot2, KeyCode.Alpha2);
            _controls.Add(InputControl.Slot3, KeyCode.Alpha3);
            _controls.Add(InputControl.Slot4, KeyCode.Alpha4);
            _controls.Add(InputControl.Slot5, KeyCode.Alpha5);
            _controls.Add(InputControl.Slot6, KeyCode.Alpha6);
            _controls.Add(InputControl.Slot7, KeyCode.Alpha7);
            _controls.Add(InputControl.Slot8, KeyCode.Alpha8);
            _controls.Add(InputControl.Slot9, KeyCode.Alpha9);
            _controls.Add(InputControl.Slot10, KeyCode.Alpha0);
            _controls.Add(InputControl.Tap, KeyCode.D);
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
            return Input.GetKeyDown(_controls[InputControl.InGameMenu]);
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
    public static Dictionary<InputControl, KeyCode> ControlTableCopy()
    {
        Dictionary<InputControl, KeyCode> copy = new Dictionary<InputControl, KeyCode>();
        foreach (InputControl key in _controls.Keys)
        {
            copy.Add(key, _controls[key]);
        }
        return copy;
    }

    public static KeyCode GetKeyCode()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return KeyCode.None;
    }
}
