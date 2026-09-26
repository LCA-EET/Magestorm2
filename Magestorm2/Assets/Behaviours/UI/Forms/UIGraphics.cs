using UnityEngine;

public class UIGraphics : ValidatableForm
{
    public MultiOption Shadows;
    private int _initialShadowSetting;
    private void Start()
    {
        _initialShadowSetting = PlayerPrefs.GetInt(GameSettings.Shadows);
        Shadows.SetOption(_initialShadowSetting);
        AssociateFormToButtons();
    }
    protected override void PassedValidation()
    {
        if(_initialShadowSetting != Shadows.SelectedOption)
        {
            PlayerPrefs.SetInt(GameSettings.Shadows, Shadows.SelectedOption);
            GameSettings.ResetLSR();
            if (ComponentRegister.Scene != null)
            {
                GameSettings.ApplyLightShadowSetting();
            }
            PlayerPrefs.Save();
        }
        CloseForm();
    }
}
