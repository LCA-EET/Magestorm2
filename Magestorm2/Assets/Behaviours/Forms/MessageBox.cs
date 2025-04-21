using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageBox : ValidatableForm
{
    private TMP_Text _textBox;
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
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        CloseForm();
    }
}
