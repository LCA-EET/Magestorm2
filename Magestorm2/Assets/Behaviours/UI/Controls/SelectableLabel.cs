using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableLabel : MonoBehaviour
{
    public Button Button;
    public TMP_Text Caption;

    private int _optionID;
    private ILabelCollection _owner;
    public void Register(int referenceID, int optionID, ILabelCollection owner)
    {
        _optionID = optionID;
        Caption.text = Language.GetBaseString(referenceID);
        Button.onClick.AddListener(ButtonPressed);
        _owner = owner;
        gameObject.SetActive(true);
    }

    private void ButtonPressed()
    {
        _owner.RecordSelection(_optionID);
    }
}
