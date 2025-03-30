using UnityEngine;
using UnityEngine.UI;
public enum ButtonType : byte
{
    Submit = 0,
    Cancel = 1
}
public class FormButton : MonoBehaviour
{
    public ButtonType buttonType;
    private ValidatableForm _associatedForm;
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(NotifyForm);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
