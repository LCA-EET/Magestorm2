using UnityEngine;

public class UIGraphics : ValidatableForm
{
    private MultiOption[] _options;
    private void Awake()
    {
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
        }
        PlayerPrefs.Save();
        CloseForm();
    }
}
