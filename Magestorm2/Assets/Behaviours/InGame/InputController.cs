using System.Collections.Generic;
using UnityEngine;
public class InputController : MonoBehaviour
{
    private HashSet<ControlInput> _gameModeControls = new HashSet<ControlInput>();
    private Dictionary<InputControl, bool> _controlInputs = new Dictionary<InputControl, bool>();

    private void Awake()
    {
        ComponentRegister.InputController = this;
    }
    private void Start()
    {
        _controlInputs = new Dictionary<InputControl, bool>();

        _gameModeControls.Add(new ControlInput(InputControl.Forward, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.Backward, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.StrafeLeft, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.StrafeRight, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.Ascend, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.Descend, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.Run, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.Jump, KeyMode.KeyPressed));
        _gameModeControls.Add(new ControlInput(InputControl.Crouch, KeyMode.KeyDown));

        foreach (ControlInput ci in _gameModeControls)
        {
            _controlInputs.Add(ci.InputControl, false);
        }
    }
    public bool IsSet(InputControl control, bool gameMode)
    {
        return _controlInputs[control] && gameMode;
    }
    public void RefreshKeyCodes()
    {
        foreach(ControlInput input in _gameModeControls)
        {
            input.RefreshKeyCode();
        }
    }
    private void Update()
    {
        if (Game.GameMode)
        {
            foreach (ControlInput control in _gameModeControls)
            {
                _controlInputs[control.InputControl] = control.GetResult();
            }
        }
        
    }

}
