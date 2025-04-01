using UnityEngine;

public class LanguageSelector : ValidatableForm
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        Debug.Log(buttonType.ToString());
        switch (buttonType)
        {
            case ButtonType.English:
                Language.SetLanguage(Languages.English);
                break;
            case ButtonType.Russian:
                Language.SetLanguage(Languages.Russian);
                break;
            case ButtonType.Spanish:
                break;
            case ButtonType.Chinese:
                break;
        }
    }
}
