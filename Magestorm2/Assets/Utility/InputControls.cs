using UnityEngine;
using System.Collections;
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
    Descend = 10
}
public static class InputControls
{
    private static Dictionary<InputControl, KeyCode> _controls;
    public static void Init()
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
    }
    public static bool Run
    {
        get
        {
            return (Input.GetKey(_controls[InputControl.Run]) && !Backward);
        }
    }
    public static bool Jump
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Jump]);
        }
    }
    public static bool ShootPrimary
    {
        get
        {
            return Input.GetKey(_controls[InputControl.ShootPrimary]);
        }
    }
    public static bool Forward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Forward]);
        }
    }
    public static bool Backward
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Backward]);
        }
    }
    public static bool StrafeLeft
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeLeft]);
        }
    }
    public static bool StrafeRight
    {
        get
        {
            return Input.GetKey(_controls[InputControl.StrafeRight]);
        }
    }
    public static bool Ascend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Ascend]);
        }
    }
    public static bool Descend
    {
        get
        {
            return Input.GetKey(_controls[InputControl.Descend]);
        }
    }
    public static bool IsPressed(InputControl key)
    {
        return Input.GetKey(_controls[key]);
    }
}
