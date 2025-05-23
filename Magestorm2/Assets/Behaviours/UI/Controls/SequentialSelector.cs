using Unity.VisualScripting;
using UnityEngine;

public class SequentialSelector : ValidatableForm
{
    private UIModelPreview _owner;
    private byte _options;
    private int _index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _index = 0;
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Increase:
                _index++;
                break;
            case ButtonType.Decrease:
                _index--;
                break;
        }
        if(_index > (_options - 1))
        {
            _index = 0;
        }
        else if (_index < 0)
        {
            _index = (byte)(_options - 1);
        }
        _owner.SelectionChanged();
    }
    public void SetOptionCount(byte options)
    {
        _options = options;
        if(_index >= _options)
        {
            _index = 0;
        }
    }
    public void AssignOwner(UIModelPreview owner)
    {
        _owner = owner;
    }
    public byte SelectedIndex
    {
        get { return (byte)_index; }
    }
}
