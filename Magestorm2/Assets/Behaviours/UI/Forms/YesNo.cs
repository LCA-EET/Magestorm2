using TMPro;
using UnityEngine;

public class YesNo : ValidatableForm
{
    private TMP_Text _textBox;
    private ValidatableForm _instantiator;

    private void Awake()
    {
        _textBox = GetComponentInChildren<TMP_Text>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void SetParams(object[] paramArray)
    {
        _textBox.text = paramArray[0].ToString();
        _instantiator = (ValidatableForm)paramArray[1];
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Submit:
                _instantiator.SetResult(FormResult.Yes);
                break;
            case ButtonType.Cancel:
                _instantiator.SetResult(FormResult.No);
                break;
        }
        base.CloseForm();
    }
}
