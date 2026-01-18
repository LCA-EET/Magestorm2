import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.ArrayList;

public class PregamePacketProcessor extends UDPProcessor
{
    private int _accountID;

    public PregamePacketProcessor(int port){
        super(port);
    }

    @Override
    protected boolean ProcessPacket(DatagramPacket received) {
        PreProcess(received);
        _accountID = IsLoggedIn();

        if(_accountID > 0){
            switch (_opCode) {
                case Pregame_Receive.CreateCharacter:
                    HandleCreateCharacterPacket();
                    break;
                case Pregame_Receive.LogOut:
                    GameServer.ClientLoggedOut(_accountID);
                    break;
                case Pregame_Receive.DeleteCharacter:
                    HandleDeleteCharacterPacket();
                    break;
                case Pregame_Receive.SubscribeToMatches:
                    HandleMatchSubscribePacket(true, _remote);
                    break;
                case Pregame_Receive.UnsubscribeFromMatches:
                    HandleMatchSubscribePacket(false, _remote);
                    break;
                case Pregame_Receive.CreateMatch:
                    HandleMatchCreatedPacket();
                    break;
                case Pregame_Receive.DeleteMatch:
                    MatchManager.DeleteMatch(_accountID, _remote);
                    break;
                case Pregame_Receive.RequestLevelsList:
                    Main.LogMessage("Level Request Received from " + _remote.AccountID());
                    EnqueueForSend(Packets.LevelListPacket(), _remote);
                    break;
                case Pregame_Receive.RequestMatchDetails:
                    HandleMatchDetailsPacket();
                    break;
                case Pregame_Receive.NameCheck:
                    HandleNameCheckPacket();
                    break;
                case Pregame_Receive.UpdateAppearance:
                    HandleAppearanceUpdatePacket();
                    break;
                case Pregame_Receive.JoinMatch:
                    HandleJoinMatchPacket();
                    break;
                case Pregame_Receive.RequestMatchList:
                    MatchManager.SendMatchListToClient(_remote);
                    break;
                case Pregame_Receive.UpdateSkills:
                    HandleSkillUpdate();
                    break;
                case Pregame_Receive.UpdateSkillsAndSlotting:
                    HandleSlotAndSkillUpdate();
                    break;
            }
        }
        else{
            Main.LogMessage("OpCode: " + _opCode + ". Not logged in.");
            if(_opCode == Pregame_Receive.LogIn){
                HandleLogInPacket();
            }
            else if (_opCode == Pregame_Receive.CreateAccount){
                HandleCreateAccountPacket();
            }
        }
        return true;
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
            EnqueueForSend(Cryptographer.Encrypt(_decrypted), _remote);
        }
    }
    private void HandleJoinMatchPacket()
    {
        byte matchID = _decrypted[5];
        byte teamID = _decrypted[6];
        Match toJoin = MatchManager.GetMatch(matchID);
        if(toJoin != null){
            if(toJoin.HasRoomForAnotherPlayer()){
                RemoteClient remote = GameServer.GetClient(_accountID);
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
    }
    public void HandleNameCheckPacket(){
        byte nameLength = _decrypted[5];
        String toCheck = ByteUtils.BytesToUTF8(_decrypted, 6, nameLength);
        EnqueueForSend(Packets.NameCheckResults(Database.CheckIfNameIsUsed(toCheck)), _remote);
    }
    public void HandleMatchDetailsPacket(){
        byte matchID = _decrypted[5];
        Match match = MatchManager.GetMatch(matchID);
        if(match != null){
            EnqueueForSend(Packets.MatchDetailsPacket(match), _remote);
        }
    }
    public void HandleMatchCreatedPacket(){
        Main.LogMessage("Account " + _accountID + " is creating a match.");
        byte sceneID = _decrypted[5];
        byte duration = _decrypted[6];
        byte matchType = _decrypted[7];
        byte[] matchOptions = new byte[_decrypted.length - 8];
        if(matchOptions.length > 0){
            System.arraycopy(_decrypted, 8, matchOptions, 0, matchOptions.length);
        }
        MatchManager.RequestMatchCreation(_accountID, sceneID, duration, matchType, matchOptions);
    }

    public void HandleMatchSubscribePacket(boolean subscribe, RemoteClient remote){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        MatchManager.Subscribe(_accountID, subscribe, characterID, remote);
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
        byte[] toSend;
        if(validCreds){
            if(GameServer.IsLoggedIn(accountID)){
                toSend = Packets.AlreadyLoggedInPacket();
                RemoteClient alreadyExisting = GameServer.ClientLoggedOut(accountID);
                if(alreadyExisting != null){
                    EnqueueForSend(Packets.RemovedFromServerPacket(RemovalReason.AlreadyLoggedIn),
                            alreadyExisting);
                }
            }
            else {
                _remote.SetNameAndID(username, accountID);
                GameServer.ClientLoggedIn(_remote);
                toSend = Packets.LoginSucceededPacket(accountID);
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
                long token = Cryptographer.RandomToken();
                boolean accountCreated = Database.CreateAccount(username, creds[1], email, token);
                byte[] toSend = accountCreated? Packets.AccountCreatedPacket(): Packets.CreationFailedPacket();
                EnqueueForSend(toSend, _remote);
                String activationMessage = "Hello<br><br>Click the following link to activate your Magus account:<br><a href='https://www.fosiemods.net/ms2.php?appid=ms2&func=activate&activationtoken=" + token + "'>Activation Link</a>";
                Main.Mailer.SendMail(email, "Magus Account Activation Link", activationMessage, "Magus Activation");
            }
        }
        else{
            EnqueueForSend(Packets.ProhibitedLanguagePacket(Pregame_Send.ProhibitedLanguage), _remote);
        }

    }

}
