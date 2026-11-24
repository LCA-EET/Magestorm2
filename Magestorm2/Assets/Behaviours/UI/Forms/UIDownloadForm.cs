using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
namespace Magus
{
    public class UIDownloadForm : ValidatableForm
    {
        public TMP_Text MessageText;
        void Start()
        {
            AssociateFormToButtons();
            MessageText.text = Language.BuildString(459, Game.RequiredVersion, Game.DownloadURL);
        }
        public override void ButtonPressed(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Submit:
                    try
                    {
                        System.Diagnostics.Process.Start(Game.DownloadURL);
                    }
                    catch (Exception ex) { }
                    break;
            }
            base.CloseForm();
        }
    }
}
