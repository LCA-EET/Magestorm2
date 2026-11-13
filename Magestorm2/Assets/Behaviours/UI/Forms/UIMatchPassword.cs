namespace Magus
{
    public class UIMatchPassword : ValidatableForm
    {
        private void Start()
        {
            AssociateFormToButtons();
        }
        protected override void PassedValidation()
        {
            CloseForm();
        }
        protected override bool ValidateForm()
        {
            if (base.ValidateForm())
            {
                string password = ((PasswordField)EntriesToValidate[0]).TextInput.text;
                string confirm = ((PasswordField)EntriesToValidate[1]).TextInput.text;
                if (password == confirm)
                {
                    MatchParams.HashedMatchPassword = Cryptographer.SHA256HashBytes(password);
                    return true;
                }
                else
                {
                    Game.MessageBoxReference(419);
                }
            }
            else
            {
                Game.MessageBoxReference(20);
            }
            return false;
        }
    }
}
