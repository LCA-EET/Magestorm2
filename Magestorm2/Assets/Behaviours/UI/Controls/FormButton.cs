using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum ButtonType : byte
{
    Submit = 0,
    Cancel = 1,
    LogIn = 2,
    CreateAccount = 3,
    English = 4,
    Russian = 5,
    Spanish = 6,
    Chinese = 7,
    Edit = 8,
    Delete = 9,
    Select = 10,
    CharacterSelect = 11,
    CreateMatch = 12,
    JoinMatch = 13,
    DeleteMatch = 14,
    Increase = 15,
    Decrease = 16,
    Misc0 = 17,
    Misc1 = 18,
    Misc2 = 19,
    Misc3 = 20
}
public class FormButton : MonoBehaviour
{
    public ButtonType buttonType;
    private ValidatableForm _associatedForm;
    private Button _button;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(NotifyForm);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public virtual void CallBack(ButtonType buttonType)
    {

    }
    private void NotifyForm()
    {
        UIAudio.PlayButtonPress();
        _associatedForm.ButtonPressed(buttonType);
    }
    public void SetForm(ValidatableForm associatedForm)
    {
        _associatedForm = associatedForm;
    }
}
