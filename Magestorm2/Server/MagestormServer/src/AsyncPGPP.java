import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Base64;

public class AsyncPGPP extends AsyncRequestProcessor{
    private int _accountID;
    public AsyncPGPP(String desc, PregamePacketProcessor pgp)
    {
        super(desc, pgp);
    }

    @Override
    public void ProcessPacket(AsyncRequest toProcess){
        super.PreProcess(toProcess);
        _accountID = ((AsyncPGPPRequest)toProcess).GetAccountID();
        switch (toProcess.GetOpCode()) {
            case Pregame_Receive.CreateCharacter ->
                HandleCreateCharacterPacket();
            case Pregame_Receive.DeleteCharacter ->
                HandleDeleteCharacterPacket();
            case Pregame_Receive.NameCheck ->
                _udp.EnqueueForSend(Packets.NameCheckResults(
                    Database.CheckIfNameIsUsed(ByteUtils.BytesToUTF8(_decrypted, 6, _decrypted[5]))), _remote);
            case Pregame_Receive.UpdateAppearance ->
                HandleAppearanceUpdatePacket();
            case Pregame_Receive.UpdateSkills ->
                HandleSkillUpdate();
            case Pregame_Receive.UpdateSkillsAndSlotting ->
                HandleSlotAndSkillUpdate();
            case Pregame_Receive.CreateAccount ->
                HandleCreateAccountPacket();
            case Pregame_Receive.LogIn ->
                HandleLogInPacket();
        };
    }
    private String[] LogInDetails(){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(_decrypted, 3);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        String[] toReturn = new String[2];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        return toReturn;
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
                            _udp.EnqueueForSend(Packets.RemovedFromServerPacket(RemovalReason.AlreadyLoggedIn),
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
        _udp.EnqueueForSend(toSend,_remote);
    }
    private void HandleCreateAccountPacket(){
        String[] creds = CreateAccountDetails();
        String username = creds[0];
        if(!ProfanityChecker.ContainsProhibitedLanguage(username)){
            String email = creds[2];
            Main.LogMessage("Account creation requested: " + username + ", " + email);
            if(Database.AccountRecordCount(username, email) > 0){
                _udp.EnqueueForSend(Packets.AccountExistsPacket(), _remote);
                Main.LogMessage("Account " + username + " already exists .");
            }
            else{
                int token = SharedFunctions.GetRandomInt(1000000, 9999999);
                int accountID = (int)Database.CreateAccount(username, creds[1], email, token);
                Main.LogMessage("Account " + accountID + " created.");
                if(accountID > 0){
                    SendOTPEmail(email, token);
                    _udp.EnqueueForSend(Packets.OTPRequestPacket(accountID, email), _remote);
                }
                else{
                    _udp.EnqueueForSend(Packets.CreationFailedPacket(), _remote);
                }
            }
        }
        else{
            _udp.EnqueueForSend(Packets.ProhibitedLanguagePacket(Pregame_Send.ProhibitedLanguage), _remote);
        }
    }
    private String[] CreateAccountDetails(){
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
        Main.Mailer.SendMail(email, "Magus Account Activation Code", activationMessage, "mtasupport@mta.gg");
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
            _udp.EnqueueForSend(_decrypted, _remote);
        }
    }
    private void HandleAppearanceUpdatePacket(){
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
    private void HandleDeleteCharacterPacket(){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        Main.LogMessage("Deactivating character: " + characterID);
        Database.DeactivateCharacter(characterID, _accountID);
        _udp.EnqueueForSend(Packets.CharacterDeletedPacket(characterID), _remote);
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
            _udp.EnqueueForSend(Packets.ProhibitedLanguagePacket(Pregame_Send.ProhibitedLanguage), _remote);
        }
        else{
            if(Database.SeeIfCharacterExists(characterName)){
                _udp.EnqueueForSend(Packets.CharacterExistsPacket(), _remote);
            }
            else{
                int charID = Database.AddCharacter(_accountID, characterName, classCode, stats, appearance,
                        slots, skillsInt);
                if(charID == -1){
                    _udp.EnqueueForSend(Packets.CreationFailedPacket(), _remote);
                }
                else{
                    _udp.EnqueueForSend(Packets.CharacterCreatedPacket(charID), _remote);
                }
            }
        }
    }
}
