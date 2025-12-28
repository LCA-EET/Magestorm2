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
    public void Register(int referenceID, byte optionID, ILabelCollection owner)
    {
        _optionID = optionID;
        UpdateText(referenceID);
        Button.onClick.AddListener(ButtonPressed);
        _owner = owner;
        Background.color = Colors.EntrySelected;
    }
    public void UpdateText(int newReferenceID)
    {
        Caption.text = Language.GetBaseString(newReferenceID);
    }
    private void ButtonPressed()
    {
        UIAudio.PlayButtonPress();
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
