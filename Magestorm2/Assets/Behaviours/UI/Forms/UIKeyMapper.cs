using System.Collections.Generic;
using UnityEngine;

public class UIKeyMapper : ValidatableForm
{
    private Dictionary<InputControl, KeyCode> _controlTable;
    private KeySelector[] _keySelectors;
    private int _indexToChange;
    private InputControl _controlToChange;
    private bool _listening;
    void Start()
    {
        _controlTable = InputControls.ControlTableCopy();
        AssociateFormToButtons();
        _keySelectors = GetComponentsInChildren<KeySelector>();
        for(int i =0; i< _keySelectors.Length; i++)
        {
            _keySelectors[i].SetOwningForm(this, i);
        }
    }
    public KeyCode GetKeyCode(InputControl control)
    {
        return _controlTable[control];
    }
    public void RemapControl(string desc, InputControl control, int index)
    {
        _controlToChange = control;
        _indexToChange = index;
        _listening = true;
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Misc0:
                CloseForm();
                break;
            case ButtonType.Misc1:
                ApplyDefaults();
                break;
            case ButtonType.Misc2:
                SaveKeys();
                CloseForm();
                break;
        }
    }
    private void SaveKeys()
    {
        foreach(KeySelector keySelector in _keySelectors)
        {
            PlayerPrefs.SetInt(PlayerAccount.AccountID + "key:" + keySelector.InputKey, (int)keySelector.KeyCode);
            Debug.Log("Setting key preference: " + PlayerAccount.AccountID + "key:" + keySelector.InputKey + ", " + (int)keySelector.KeyCode);
            InputControls.SetKey(keySelector.InputKey, keySelector.KeyCode);
        }
    }
    private void ApplyDefaults()
    {
        _controlTable = InputControls.GetDefaultKeys();
        foreach(KeySelector keySelector in _keySelectors)
        {
            keySelector.SetKeyText();
        }
    }
    void Update()
    {
        if (_listening)
        {
            if (Input.anyKeyDown)
            {
                KeyCode pressed = InputControls.GetKeyCode();
                _listening = false;
                _controlTable[_controlToChange] = pressed;
                _keySelectors[_indexToChange].SetKeyText();
            }
        }
    }
}

