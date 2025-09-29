using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeySelector : ValidatableForm
{
    public InputControl InputKey;
    private KeyCode _selectedKey;
    private Button _button;
    private UIKeyMapper _owner;

    public TMP_Text Descriptor;
    public TMP_Text Key;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
    }
    public void SetOwningForm(UIKeyMapper owner)
    {
        _owner = owner;
        Key.text = _owner.GetKeyCode(InputKey).ToString();
    }

}
