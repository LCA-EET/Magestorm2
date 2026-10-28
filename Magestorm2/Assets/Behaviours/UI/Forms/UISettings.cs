using UnityEngine;

public class UIGraphics : ValidatableForm
{
    private MultiOption[] _options;
    protected override void Awake()
    {
        base.Awake();
        _options = GetComponentsInChildren<MultiOption>();
    }
    private void Start()
    {
        foreach(MultiOption option in _options)
        {
            option.SetOption(GameSettings.GetSettingValue(option.SettingKey));
        }
        AssociateFormToButtons();
    }
    protected override void PassedValidation()
    {
        foreach (MultiOption option in _options)
        {
            GameSettings.SetSettingValue(option.SettingKey, option.SelectedOption);
            Debug.Log("Saving setting: " + option.SettingKey + ", " + option.SelectedOption);
        }
        GameSettings.WriteSettings();
        CloseForm();
    }
}
