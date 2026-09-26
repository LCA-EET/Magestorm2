using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIKeyMapper : ValidatableForm
{
    private Dictionary<InputControl, KeyCode> _controlTable;
    private KeySelector[] _keySelectors;
    private int _indexToChange;
    private InputControl _controlToChange;
    private bool _listening;
    private int _page;
    public GameObject[] Pages;
    void Start()
    {
        _page = 0;
        Game.ControlMode = true;
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
        if (_controlTable.ContainsKey(control))
        {
            return _controlTable[control];
        }
        else
        {
            return KeyCode.None;
        }
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
            case ButtonType.Misc3:
                Debug.Log("M3");
                DecrementPage();
                break;
            case ButtonType.Misc4:
                IncrementPage();
                break;
        }
    }
    private void ShowPage()
    {
        foreach(GameObject page in Pages)
        {
            page.SetActive(false);
        }
        Pages[_page].SetActive(true);
    }
    private void DecrementPage()
    {
        _page--;
        if(_page < 0)
        {
            _page = Pages.Length - 1;
        }
        ShowPage();
    }
    private void IncrementPage()
    {
        _page++;
        if(_page >= Pages.Length)
        {
            _page = 0;
        }
        ShowPage();
    }
    private void SaveKeys()
    {
        foreach(KeySelector keySelector in _keySelectors)
        {
            PlayerPrefs.SetInt(PlayerAccount.AccountID + "key:" + keySelector.InputKey, (int)keySelector.KeyCode);
            Debug.Log("Setting key preference: " + PlayerAccount.AccountID + "key:" + keySelector.InputKey + ", " + (int)keySelector.KeyCode);
            InputControls.SetKey(keySelector.InputKey, keySelector.KeyCode);
            if(ComponentRegister.InputController != null)
            {
                ComponentRegister.InputController.RefreshKeyCodes();
            }
        }
        if(ComponentRegister.AvailableSpellsPanel != null)
        {
            ComponentRegister.AvailableSpellsPanel.RefeshPanel();
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
        if (Game.GameMode)
        {
            if (InputControls.InGameMenu)
            {
                CloseForm();
            }
        }
        else
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
    public override void CloseForm()
    {
        Game.ControlMode = false;
        base.CloseForm();
    }
}

