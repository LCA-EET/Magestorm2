using UnityEngine;
using UnityEngine.UI;

public class PasswordField : TextField
{
    private UnityEngine.UI.Button _showTextButton;
    private bool _textShown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();
        _showTextButton = GetComponentInChildren<Button>();
        _showTextButton.onClick.AddListener(ShowTextClicked);
        TextInput.contentType = TMPro.TMP_InputField.ContentType.Password;
    }
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    private void ShowTextClicked()
    {
        _textShown = !_textShown;
        if (_textShown)
        {
            TextInput.contentType = TMPro.TMP_InputField.ContentType.Standard;
        }
        else
        {
            TextInput.contentType = TMPro.TMP_InputField.ContentType.Password;
        }
        TextInput.ForceLabelUpdate();
    }
    public override bool Validate()
    {
        
        if (!base.Validate())
        {
            return false;
        }
        else
        {
            string textInput = TextInput.text;
            byte characterTypes = 0;
            bool letterFound = false;
            bool digitFound = false;
            bool symbolFound = false;
            foreach (char c in textInput)
            {
                if (char.IsLetter(c) && !letterFound)
                {
                    letterFound = true;
                    characterTypes++; 
                }
                if (char.IsDigit(c) && !digitFound) 
                {
                    digitFound = true;
                    characterTypes++;
                }
                if (char.IsSymbol(c) && !symbolFound)
                {
                    symbolFound = true;
                    characterTypes++;
                }
                if(characterTypes >= 2)
                {
                    break;
                }
            }
            return characterTypes >= 2;
        }
    }
}
