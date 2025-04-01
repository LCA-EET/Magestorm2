using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TextField : ValidateableObject
{
    public TMP_InputField TextInput;
    public TMP_Text PlaceholderField;
    public TMP_Text Header;
    public byte MaxLength;
    public byte MinLength;
    public string RegexExpression;
    public Image InvalidEntryImage;
    private string _priorText;
    private Regex _regex;
    protected virtual void Awake()
    {
        MarkInvalid(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if(RegexExpression.Length > 0)
        {
            _regex = new Regex(RegexExpression, RegexOptions.IgnoreCase);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(_priorText != TextInput.text)
        {
            MarkInvalid(false);
        }
        _priorText = TextInput.text;
    }
    public override void MarkInvalid(bool invalid)
    {
        InvalidEntryImage.gameObject.SetActive(invalid);
    }
    public override bool Validate()
    {
        string input = TextInput.text;
        if (input.Contains(" "))
        {
            return false;
        }
        if (RegexExpression.Length > 0)
        {
            if (!_regex.Match(input).Success)
            {
                return false;
            }
        }
        return CheckLength(input);
    }
    protected bool CheckLength(string input)
    {
        return (input.Length >= MinLength) && (input.Length <= MaxLength);
    }
}
