import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.ArrayList;
import java.util.Collection;

public class PregamePacketProcessor extends UDPProcessor
{
    private int _accountID;
    public PregamePacketProcessor(int port){
        super(port, ServerParams.Encryption);
        GameServer.PGP = this;
    }
    @Override protected boolean PreProcess(DatagramPacket received)
    {
        super.PreProcess(received);
        _remote = LoggedInClient();
        if(_remote != null){
            _accountID =(int)_remote.ObjectID();
        }
        else{
            _remote = new RemoteClient(received);
        }
        return true;
    }
    @Override
    protected boolean ProcessPacket(DatagramPacket received) {
        PreProcess(received);
        switch (_opCode) {
            case Pregame_Receive.CreateCharacter ->
                HandleCreateCharacterPacket();
            case Pregame_Receive.LogOut ->
                RemoteClientManager.ClientLoggedOut(_accountID);
            case Pregame_Receive.DeleteCharacter ->
                HandleDeleteCharacterPacket();
            case Pregame_Receive.SubscribeToMatches ->
                HandleMatchSubscribePacket(true);
            case Pregame_Receive.UnsubscribeFromMatches ->
                HandleMatchSubscribePacket(false);
            case Pregame_Receive.CreateMatch ->
                MatchManager.RequestMatchCreation(_accountID, _decrypted[5], _decrypted[6], _decrypted[7], _decrypted[8]);
            case Pregame_Receive.DeleteMatch ->
                MatchManager.DeleteMatch(_accountID, _remote);
            case Pregame_Receive.RequestLevelsList ->
                EnqueueForSend(Packets.LevelListPacket(), _remote);
            case Pregame_Receive.RequestMatchDetails ->
                HandleMatchDetailsPacket();
            case Pregame_Receive.NameCheck ->
                EnqueueForSend(Packets.NameCheckResults(
                    Database.CheckIfNameIsUsed(ByteUtils.BytesToUTF8(_decrypted, 6, _decrypted[5]))), _remote);
            case Pregame_Receive.UpdateAppearance ->
                HandleAppearanceUpdatePacket();
            case Pregame_Receive.JoinMatch ->
                HandleJoinMatchPacket(_decrypted[5], _decrypted[6]);
            case Pregame_Receive.RequestMatchList ->
                MatchManager.SendMatchListToClient(_remote);
            case Pregame_Receive.UpdateSkills ->
                HandleSkillUpdate();
            case Pregame_Receive.UpdateSkillsAndSlotting ->
                HandleSlotAndSkillUpdate();
            case Pregame_Receive.RequestMatchScore ->
                SendMatchScore(_decrypted[5], _remote);
            case Pregame_Receive.RequestSpellData ->
                HandleSpellDataRequest();
            case Pregame_Receive.RequestDisciplineData ->
                EnqueueForSend(DisciplineManager.GetDisciplinePacket(), _remote);
            case Pregame_Receive.RequestCharacterClassData ->
                EnqueueForSend(CharacterClassManager.ClientDataPacket(), _remote);
            case Pregame_Receive.LogIn ->
                HandleLogInPacket();
            case Pregame_Receive.CreateAccount ->
                HandleCreateAccountPacket();
            case Pregame_Receive.OTPReply ->
                HandleOTPReply();
        }
        return true;
    }
    private void HandleSpellDataRequest(){
        Collection<byte[]> packets = SpellManager.GetSpellDataPackets();
        for(byte[] packet : packets){
            EnqueueForSend(packet, _remote);
        }
    }
    private void HandleOTPReply(){
        int accountID = ByteUtils.ExtractInt(_decrypted, 1);
        int otp = ByteUtils.ExtractInt(_decrypted, 5);
        boolean result = Database.ActivateAccount(accountID, otp);
        EnqueueForSend(Packets.ActivationResultPacket(result?(byte)1:(byte)0), _remote);
    }
    private void HandleSkillUpdate(){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        if(CharacterManager.CharacterBelongsToAccount(characterID, _accountID)){
            int skills = ByteUtils.ExtractInt(_decrypted, 9);
            Database.UpdateSkills(characterID, skills);
            CharacterManager.GetCharacter(characterID).UpdateSkills(skills);
        }
    }
    private void HandleSlotAndSkillUpdate(){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        if(CharacterManager.CharacterBelongsToAccount(characterID, _accountID)){
            int skills = ByteUtils.ExtractInt(_decrypted, 9);
            byte[] slots = new byte[10];
            System.arraycopy(_decrypted, 13, slots, 0, slots.length);
            Database.UpdateSkillsAndSlots(characterID, skills, slots);
            PlayerCharacter toUpdate = CharacterManager.GetCharacter(characterID);
            toUpdate.UpdateSkills(skills);
            toUpdate.UpdateSlottedSpells(slots);
            _decrypted[0] = Pregame_Send.UpdateSkillsAndSlots;
            EnqueueForSend(_cryptographer.Encrypt(_decrypted), _remote);
        }
    }
    private void HandleJoinMatchPacket(byte matchID, byte teamID)
    {
        Match toJoin = MatchManager.GetMatch(matchID);
        if(toJoin != null){
            if(toJoin.HasRoomForAnotherPlayer()){
                RemoteClient remote = RemoteClientManager.GetClient(_accountID);
                remote.UnsubscribeFromMatches();
                toJoin.JoinMatch(remote, teamID);
            }
            else{
                EnqueueForSend(Packets.MatchIsFullPacket(), _remote);
            }
        }
    }

    public void HandleAppearanceUpdatePacket(){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        byte[] appearanceBytes = new byte[5];
        System.arraycopy(_decrypted, 9, appearanceBytes, 0, appearanceBytes.length);
        Database.UpdateCharacterAppearance(characterID, appearanceBytes);
        PlayerCharacter toUpdate = CharacterManager.GetCharacter(characterID);
        if(toUpdate != null){
            Main.LogMessage("Updating appearance for character " + characterID);
            toUpdate.UpdateAppearanceBytes(appearanceBytes);
        }
    }
    public void HandleMatchDetailsPacket(){
        byte matchID = _decrypted[5];
        Match match = MatchManager.GetMatch(matchID);
        if(match != null){
            if(match.IsOptionEnabled(ControlCodes.MatchOptions_AntiStack)){
                byte teamToJoin = match.GetASTeam();
                HandleJoinMatchPacket(matchID, teamToJoin);
            }
            else{
                EnqueueForSend(Packets.MatchDetailsPacket(match), _remote);
            }
        }
    }
    public void HandleMatchSubscribePacket(boolean subscribe){
        RemoteClient rc = MatchManager.Subscribe(_accountID, subscribe, ByteUtils.ExtractInt(_decrypted, 5));
        byte priorMatchID = _decrypted[9];
        if(_decrypted[10] == 1){ // qm
            HandleJoinMatchPacket(MatchManager.GetQMID(), (byte)0);
        }
        else if (priorMatchID != 0){
            SendMatchScore(priorMatchID, rc);
        }
    }
    public String[] LogInDetails(){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(_decrypted, 3);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        String[] toReturn = new String[2];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        return toReturn;
    }
    private void HandleDeleteCharacterPacket(){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        Main.LogMessage("Deactivating character: " + characterID);
        Database.DeactivateCharacter(characterID, _accountID);
        EnqueueForSend(Packets.CharacterDeletedPacket(characterID), _remote);
    }
    private void SendMatchScore(byte matchID, RemoteClient rc){
        byte[] toEncrypt= MatchManager.GetScoreBytes(matchID);
        EnqueueForSend(Packets.MatchScoresPacket(Pregame_Send.MatchScore, toEncrypt == null ? new byte[2]:toEncrypt), rc);
    }
    private void HandleCreateCharacterPacket(){
        byte classCode = _decrypted[5];
        byte[] stats = new byte[6];
        byte[] appearance = new byte[5];
        System.arraycopy(_decrypted, 6, stats, 0, 6);
        System.arraycopy(_decrypted, 12, appearance, 0, 5);
        if(AntiCheat.CheckStats(stats, _remote, _accountID)){
            return;
        }
        byte[] slots = new byte[10];
        System.arraycopy(_decrypted, 17, slots, 0, slots.length);
        int skillsInt = ByteUtils.ExtractInt(_decrypted, 27);
        byte nameLength = _decrypted[31];
        String characterName = new String(Packets.ExtractBytes(_decrypted, 32, nameLength),
                StandardCharsets.UTF_8);
        if(ProfanityChecker.ContainsProhibitedLanguage(characterName)){
            EnqueueForSend(Packets.ProhibitedLanguagePacket(Pregame_Send.ProhibitedLanguage), _remote);
        }
        else{
            if(Database.SeeIfCharacterExists(characterName)){
                EnqueueForSend(Packets.CharacterExistsPacket(), _remote);
            }
            else{
                int charID = Database.AddCharacter(_accountID, characterName, classCode, stats, appearance,
                        slots, skillsInt);
                if(charID == -1){
                    EnqueueForSend(Packets.CreationFailedPacket(), _remote);
                }
                else{
                    EnqueueForSend(Packets.CharacterCreatedPacket(charID), _remote);
                }
            }
        }
    }

    private void HandleLogInPacket(){
        String[] creds = LogInDetails();
        String username = creds[0];
        String hashed = creds[1];
        Object[] validationResult = Database.ValidateCredentials(username, hashed);
        boolean validCreds = (boolean)validationResult[0];
        int accountID = (int)validationResult[1];
        byte accountStatus = (byte)validationResult[2];
        String email = validationResult[3].toString();
        int otp = (int)validationResult[4];
        byte[] toSend;
        if(validCreds){
            switch(accountStatus){
                case ControlCodes.AccountStatus_Active:
                    if(RemoteClientManager.IsLoggedIn(accountID)){
                        toSend = Packets.AlreadyLoggedInPacket();
                        RemoteClient alreadyExisting = RemoteClientManager.ClientLoggedOut(accountID);
                        if(alreadyExisting != null){
                            EnqueueForSend(Packets.RemovedFromServerPacket(RemovalReason.AlreadyLoggedIn),
                                    alreadyExisting);
                        }
                    }
                    else {
                        _remote.SetNameAndID(username, accountID);
                        RemoteClientManager.ClientLoggedIn(_remote);
                        toSend = Packets.LoginSucceededPacket(accountID);
                        MatchManager.RemoveCharactersForAccount(accountID);
                    }
                    break;
                case ControlCodes.AccountStatus_NotActivated:
                    SendOTPEmail(email, otp);
                    toSend = Packets.OTPRequestPacket(accountID, email);
                    break;
                default:
                    toSend = Packets.AccountBannedPacket();
                    break;
            }
        }
        else{
            toSend = Packets.LoginFailedPacket();
        }
        EnqueueForSend(toSend,_remote);
    }

    public String[] CreateAccountDetails(){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(_decrypted, 4);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        byte[] emailBytes = toProcess.get(2);
        String[] toReturn = new String[3];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        toReturn[2] = new String(emailBytes, StandardCharsets.UTF_8);
        return toReturn;
    }
    private void SendOTPEmail(String email, int otp){
        String activationMessage = "Hello<br><br>Enter the following OTP into the Magus client to activate your account: " + otp;
        Main.Mailer.SendMail(email, "Magus Account Activation Code", activationMessage, "Magus Activation");
    }
    private void HandleCreateAccountPacket(){
        String[] creds = CreateAccountDetails();
        String username = creds[0];
        if(!ProfanityChecker.ContainsProhibitedLanguage(username)){
            String email = creds[2];
            Main.LogMessage("Account creation requested: " + username + ", " + email);
            if(Database.AccountRecordCount(username, email) > 0){
                EnqueueForSend(Packets.AccountExistsPacket(), _remote);
                Main.LogMessage("Account " + username + " already exists .");
            }
            else{
                Main.LogMessage("Account " + username + " does not already exist.");
                int token = SharedFunctions.GetRandomInt(1000000, 9999999);
                int accountID = (int)Database.CreateAccount(username, creds[1], email, token);
                Main.LogMessage("Account " + accountID + " created.");
                if(accountID > 0){
                    SendOTPEmail(email, token);
                    EnqueueForSend(Packets.OTPRequestPacket(accountID, email), _remote);
                }
                else{
                    EnqueueForSend(Packets.CreationFailedPacket(), _remote);
                }
            }
        }
        else{
            EnqueueForSend(Packets.ProhibitedLanguagePacket(Pregame_Send.ProhibitedLanguage), _remote);
        }
    }
}
