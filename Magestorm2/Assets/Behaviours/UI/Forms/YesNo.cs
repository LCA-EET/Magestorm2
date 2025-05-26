using TMPro;
using UnityEngine;

public class YesNo : ValidatableForm
{
    public TMP_Text MessageText;
    private ValidatableForm _instantiator;

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
        MessageText.text = paramArray[0].ToString();
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
