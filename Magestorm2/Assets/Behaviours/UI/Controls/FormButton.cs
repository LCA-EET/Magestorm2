using TMPro;
using UnityEngine;
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
    Decrease = 16
}
public class FormButton : MonoBehaviour
{
    public ButtonType buttonType;
    private ValidatableForm _associatedForm;
    private Button _button;
    void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(NotifyForm);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void NotifyForm()
    {
        _associatedForm.ButtonPressed(buttonType);
    }
    public void SetForm(ValidatableForm associatedForm)
    {
        _associatedForm = associatedForm;
    }
}
