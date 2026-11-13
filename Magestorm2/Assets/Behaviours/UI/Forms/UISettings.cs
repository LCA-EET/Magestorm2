using UnityEngine;
namespace Magus
{

    public class UIGraphics : ValidatableForm
    {
        private MultiOption[] _options;
        private int _page;
        public GameObject[] Pages;
        
        private void Start()
        {
            AssociateFormToButtons();
            ShowPage();
            FillSettings();
        }
        protected override void PassedValidation()
        {
            for(int i = 0; i < Pages.Length; i++)
            {
                _page = i;
                ShowPage();
                _options = GetComponentsInChildren<MultiOption>();
                foreach (MultiOption option in _options)
                {
                    GameSettings.SetSettingValue(option.SettingKey, option.SelectedOption);
                    Debug.Log("Saving setting: " + option.SettingKey + ", " + option.SelectedOption);
                }
            }
            GameSettings.WriteSettings();
            CloseForm();
        }
        private void DecrementPage()
        {
            _page--;
            if (_page < 0)
            {
                _page = Pages.Length - 1;
            }
            ShowPage();
            FillSettings();
        }
        private void IncrementPage()
        {
            _page++;
            if (_page >= Pages.Length)
            {
                _page = 0;
            }
            ShowPage();
            FillSettings();
        }
        private void FillSettings()
        {
            _options = GetComponentsInChildren<MultiOption>();
            foreach (MultiOption option in _options)
            {
                option.SetOption(GameSettings.GetSettingValue(option.SettingKey));
            }
        }
        private void ShowPage()
        {
            foreach (GameObject page in Pages)
            {
                page.SetActive(false);
            }
            Pages[_page].SetActive(true);
        }
        public override void ButtonPressed(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Submit:
                    if (ValidateForm())
                    {
                        PassedValidation();
                    }
                    break;
                case ButtonType.Cancel:
                    CloseForm();
                    break;
                case ButtonType.Misc0:
                    DecrementPage();
                    break;
                case ButtonType.Misc1:
                    IncrementPage();
                    break;
            }
        }
    }

}