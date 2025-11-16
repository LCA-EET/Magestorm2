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
    //Tap = 26,
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
            Dictionary<InputControl, KeyCode> defaults = GetDefaultKeys();
            foreach (InputControl control in defaults.Keys)
            {
                _controls.Add(control, defaults[control]);
            }
            Dictionary<InputControl, KeyCode> keysToUpdate = new Dictionary<InputControl, KeyCode>();
            foreach (InputControl playerfunction in _controls.Keys)
            {
                string stringKey = PlayerAccount.AccountID + "key:" + playerfunction;
                if (PlayerPrefs.HasKey(stringKey))
                {
                    //Debug.Log("Loading preference " + stringKey + ", " + (KeyCode)PlayerPrefs.GetInt(stringKey));
                    keysToUpdate.Add(playerfunction, (KeyCode)PlayerPrefs.GetInt(stringKey));
                }
            }
            foreach (InputControl control in keysToUpdate.Keys)
            {
                _controls[control] = keysToUpdate[control];
            }
            _init = true;
        }
    }
    public static void SetKey(InputControl control, KeyCode key)
    {
        _controls[control] = key;
    }
    public static Dictionary<InputControl, KeyCode> GetDefaultKeys()
    {
        Dictionary<InputControl, KeyCode> defaults = new Dictionary<InputControl, KeyCode>();
        defaults.Add(InputControl.Forward, KeyCode.W);
        defaults.Add(InputControl.Backward, KeyCode.S);
        defaults.Add(InputControl.StrafeLeft, KeyCode.Q);
        defaults.Add(InputControl.StrafeRight, KeyCode.E);
        defaults.Add(InputControl.Run, KeyCode.LeftShift);
        defaults.Add(InputControl.Jump, KeyCode.Space);
        defaults.Add(InputControl.ShootPrimary, KeyCode.Mouse0);
        defaults.Add(InputControl.ShootSecondary, KeyCode.Mouse1);
        defaults.Add(InputControl.Action, KeyCode.Return);
        defaults.Add(InputControl.Ascend, KeyCode.PageUp);
        defaults.Add(InputControl.Descend, KeyCode.PageDown);
        defaults.Add(InputControl.HUDToggle, KeyCode.H);
        defaults.Add(InputControl.ChatMode, KeyCode.Quote);
        defaults.Add(InputControl.CancelChat, KeyCode.Escape); //Escape
        defaults.Add(InputControl.SendMessage, KeyCode.Return);
        defaults.Add(InputControl.PreviousTrack, KeyCode.Minus);
        defaults.Add(InputControl.NextTrack, KeyCode.Plus);
        defaults.Add(InputControl.ToggleMusic, KeyCode.M);
        defaults.Add(InputControl.ChatScrollUp, KeyCode.PageUp);
        defaults.Add(InputControl.ChatScrollDown, KeyCode.PageDown);
        defaults.Add(InputControl.ChatScrollTop, KeyCode.Home);
        defaults.Add(InputControl.ChatScrollBottom, KeyCode.End);
        defaults.Add(InputControl.MiniMapZoomIn, KeyCode.LeftBracket);
        defaults.Add(InputControl.MiniMapZoomOut, KeyCode.RightBracket);
        defaults.Add(InputControl.MiniMapZoomDefault, KeyCode.Backslash);
        defaults.Add(InputControl.InGameMenu, KeyCode.Escape); //Escape
        defaults.Add(InputControl.Slot1, KeyCode.Alpha1);
        defaults.Add(InputControl.Slot2, KeyCode.Alpha2);
        defaults.Add(InputControl.Slot3, KeyCode.Alpha3);
        defaults.Add(InputControl.Slot4, KeyCode.Alpha4);
        defaults.Add(InputControl.Slot5, KeyCode.Alpha5);
        defaults.Add(InputControl.Slot6, KeyCode.Alpha6);
        defaults.Add(InputControl.Slot7, KeyCode.Alpha7);
        defaults.Add(InputControl.Slot8, KeyCode.Alpha8);
        defaults.Add(InputControl.Slot9, KeyCode.Alpha9);
        defaults.Add(InputControl.Slot10, KeyCode.Alpha0);
        return defaults;
    }
   
    public static bool Action
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.Action]) ) && Game.GameMode; 
        }
    }
    public static bool MiniMapZoomIn
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.MiniMapZoomIn])) && Game.GameMode;
        }
    }

    public static bool MiniMapZoomOut
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.MiniMapZoomOut])) && Game.GameMode;
        }
    }
    public static bool ChatScrollTop
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollTop])) && !Game.MenuMode;
        }
    }
    public static bool ChatScrollBottom
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollBottom])) && !Game.MenuMode;
        }
    }
    public static bool ChatScrollUp
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollUp])) && !Game.MenuMode;
        }
    }
    public static bool ChatScrollDown
    {
        get
        {
            return (Input.GetKeyDown(_controls[InputControl.ChatScrollDown])) && !Game.MenuMode;
        }
    }
    public static bool Run
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.Run]) && !Backward) && Game.GameMode;
        }
    }
    public static bool Jump
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Jump]) && Game.GameMode;
        }
    }
    public static bool ShootPrimary
    {
        get
        {
            return Input.GetKey(_controls[InputControl.ShootPrimary]) && Game.GameMode;
        }
    }
    public static bool Forward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Forward]) && Game.GameMode;
        }
    }
    public static bool Backward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Backward]) && Game.GameMode;
        }
    }
    public static bool StrafeLeft
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeLeft]) && Game.GameMode;
        }
    }
    public static bool StrafeRight
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeRight]) && Game.GameMode;
        }
    }
    public static bool Ascend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Ascend]) && Game.GameMode;
        }
    }
    public static bool Descend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Descend]) && Game.GameMode;
        }
    }
    public static bool HUD
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.HUDToggle]) && Game.GameMode;
        }
    }
    public static bool SendMessage
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.SendMessage]) && Game.ChatMode;
        }
    }
    public static bool ChatMode
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.ChatMode]) && Game.GameMode;
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
            return Input.GetKeyDown(_controls[InputControl.CancelChat]) && Game.ChatMode;
        }
    }
    public static bool ToggleMusic
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.ToggleMusic]) && Game.GameMode;
        }
    }
    public static bool PreviousTrack
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.PreviousTrack]) && Game.GameMode;
        }
    }
    public static bool NextTrack
    {
        get
        {
            return Input.GetKeyDown(_controls[InputControl.NextTrack]) && Game.GameMode;
        }
    }
    public static bool IsPressed(InputControl key)
    {
        return Input.GetKey(_controls[key]) && Game.GameMode;
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
