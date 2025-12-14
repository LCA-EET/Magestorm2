using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableLabel : MonoBehaviour
{
    public Button Button;
    public TMP_Text Caption;
    public Image Background;

    private int _optionID;
    private ILabelCollection _owner;
    public void Register(int referenceID, int optionID, ILabelCollection owner)
    {
        _optionID = optionID;
        Caption.text = Language.GetBaseString(referenceID);
        Button.onClick.AddListener(ButtonPressed);
        _owner = owner;
        gameObject.SetActive(true);
        Background.color = Colors.EntrySelected;
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

    public int OptionID
    {
        get { return _optionID; }
    }
}
