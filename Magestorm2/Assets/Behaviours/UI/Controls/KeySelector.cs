using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class KeySelector : ValidatableForm
{
    public InputControl InputKey;
    private KeyCode _selectedKey;
    private UIKeyMapper _owner;
    private KeyCode _keyCode;
    private int _index;

    public TMP_Text Descriptor;
    public TMP_Text Key;

    private void Awake()
    {
    }
    public void SetKeyText()
    {
        _keyCode = _owner.GetKeyCode(InputKey);
        Key.text = _keyCode.ToString();
    }
    public void SetOwningForm(UIKeyMapper owner, int index)
    {
        _owner = owner;
        _index = index;
        SetKeyText();
        AssociateFormToButtons();
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Submit:
                _keyCode = KeyCode.None;
                Key.text = "[UNSET]";
                _owner.RemapControl(Descriptor.text, InputKey, _index);
                break;
        }
    }

    public KeyCode KeyCode
    {
        get { return _keyCode; }
    }
}
