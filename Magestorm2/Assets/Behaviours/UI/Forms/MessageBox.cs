using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageBox : ValidatableForm
{
    private TMP_Text _textBox;
    private Action _function;
    private void Awake()
    {
        _textBox = GetComponentInChildren<TMP_Text>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void SetParams(object[] paramArray)
    {
        _textBox.text = paramArray[0].ToString();
        if(paramArray.Length > 1)
        {
            _function = (Action)paramArray[1];
        }
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        CloseForm();
        if (_function != null)
        {
            _function();
        }
    }
}
