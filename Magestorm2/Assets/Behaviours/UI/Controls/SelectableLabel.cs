using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableLabel : MonoBehaviour
{
    public Button Button;
    public TMP_Text Caption;
    public Image Background;

    private byte _optionID;
    private ILabelCollection _owner;
    private bool _registered;
    public void Register(int referenceID, byte optionID, ILabelCollection owner)
    {
        _optionID = optionID;
        UpdateText(referenceID);
        if (!_registered)
        {
            Button.onClick.AddListener(ButtonPressed);
            _registered = true;
        }
        _owner = owner;
        Background.color = Colors.EntrySelected;
    }
    public void UpdateText(int newReferenceID)
    {
        Caption.text = Language.GetBaseString(newReferenceID);
        gameObject.SetActive(true);
    }
    private void ButtonPressed()
    {
        Game.UIAudio.PlayButtonPress();
        _owner.RecordSelection(_optionID);
    }

    public void MarkSelected(bool selected)
    {
        Background.gameObject.SetActive(selected);
    }

    public byte OptionID
    {
        get { return _optionID; }
    }
}
