using System;
using UnityEngine;
public enum KeyMode
{
    KeyPressed = 0,
    KeyUp = 1,
    KeyDown = 2
}
public class ControlInput
{
    private InputControl _inputControl;
    private KeyCode _toCheck;
    private Func<bool> _func;
    public ControlInput(InputControl inputControl, KeyMode keyMode)
    {
        _inputControl = inputControl;
        RefreshKeyCode();
        switch (keyMode)
        {
            case KeyMode.KeyPressed:
                _func = GetKeyResult;
                break;
            case KeyMode.KeyUp:
                _func = GetKeyUpResult;
                break;
            case KeyMode.KeyDown:
                _func = GetKeyDownResult;
                break;
        }
    }
    public void RefreshKeyCode()
    {
        _toCheck = InputControls.GetKeyCode(_inputControl);
    }
    public InputControl InputControl
    {
        get
        {
            return _inputControl;
        }
    }
    public bool GetResult()
    {
        return _func();
    }
    private bool GetKeyResult()
    {
        return Input.GetKey(_toCheck);
    }
    private bool GetKeyDownResult()
    {
        return Input.GetKeyDown(_toCheck);
    }
    private bool GetKeyUpResult()
    {
        return Input.GetKeyUp(_toCheck);
    }
}
