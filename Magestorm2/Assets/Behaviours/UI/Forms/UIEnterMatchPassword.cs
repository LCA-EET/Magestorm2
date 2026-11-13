namespace Magus
{
    public class UIEnterMatchPassword : ValidatableForm
    {
        private byte _matchID, _selectedTeam;
        public void InitForm(byte matchID, byte selectedTeam)
        {
            _matchID = matchID;
            _selectedTeam = selectedTeam;
        }

        protected override void PassedValidation()
        {
            MatchParams.HashedMatchPassword = Cryptographer.SHA256HashBytes(((PasswordField)EntriesToValidate[0]).TextInput.text);
            Game.SendPregameBytes(Pregame_Packets.JoinPrivateMatchPacket(_matchID, _selectedTeam));
        }
    }
}
