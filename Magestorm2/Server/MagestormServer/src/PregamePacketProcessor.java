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
        //Main.LogMessage(String.valueOf(_opCode));

        switch (_opCode) {
            /*
            RemoteClient Exists
             */

            case Pregame_Receive.Heartbeat:
                AssignRC();
                return true;
            case Pregame_Receive.CreateCharacter:
                AssignRC();
                HandleCreateCharacterPacket();
                break;
            case Pregame_Receive.LogOut:
                AssignRC();
                RemoteClientManager.ClientLoggedOut(_accountID);
                break;
            case Pregame_Receive.DeleteCharacter:
                AssignRC();
                HandleDeleteCharacterPacket();
                break;
            case Pregame_Receive.SubscribeToMatches:
                AssignRC();
                HandleMatchSubscribePacket(true);
                break;
            case Pregame_Receive.UnsubscribeFromMatches:
                AssignRC();
                HandleMatchSubscribePacket(false);
                break;
            case Pregame_Receive.CreateMatch:
                AssignRC();
                HandleMatchCreatedPacket();
                break;
            case Pregame_Receive.DeleteMatch:
                AssignRC();
                MatchManager.DeleteMatch(_accountID, _remote);
                break;
            case Pregame_Receive.RequestLevelsList:
                AssignRC();
                Main.LogMessage("Level Request Received from " + _remote.ObjectID());
                EnqueueForSend(Packets.LevelListPacket(), _remote);
                break;
            case Pregame_Receive.RequestMatchDetails:
                AssignRC();
                HandleMatchDetailsPacket();
                break;
            case Pregame_Receive.NameCheck:
                AssignRC();
                HandleNameCheckPacket();
                break;
            case Pregame_Receive.UpdateAppearance:
                AssignRC();
                HandleAppearanceUpdatePacket();
                break;
            case Pregame_Receive.JoinMatch:
                AssignRC();
                HandleJoinMatchPacket(_decrypted[5], _decrypted[6]);
                break;
            case Pregame_Receive.RequestMatchList:
                AssignRC();
                MatchManager.SendMatchListToClient(_remote);
                break;
            case Pregame_Receive.UpdateSkills:
                AssignRC();
                HandleSkillUpdate();
                break;
            case Pregame_Receive.UpdateSkillsAndSlotting:
                AssignRC();
                HandleSlotAndSkillUpdate();
                break;
            case Pregame_Receive.RequestCharacterData:
                AssignRC();
                HandleCharacterDataRequest();
                return true;
            case Pregame_Receive.RequestMatchScore:
                AssignRC();
                HandleMatchScoreRequest();
                return true;
               /*
            RemoteClient doesn't exist
             */
            case Pregame_Receive.LogIn:
                _remote = new RemoteClient(received);
                HandleLogInPacket();
                break;
            case Pregame_Receive.CreateAccount:
                _remote = new RemoteClient(received);
                HandleCreateAccountPacket();
                break;
        }
        return true;
    }
    private void HandleMatchScoreRequest(){
        SendMatchScore(_decrypted[5], _remote);
    }
    private void HandleQMJoin(){
        HandleJoinMatchPacket(MatchManager.GetQMID(), (byte)0);
    }
    private void AssignRC(){
        _remote = LoggedInClient();
        if(_remote != null){
            _accountID =(int)_remote.ObjectID();
        }
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
    public void HandleNameCheckPacket(){
        byte nameLength = _decrypted[5];
        String toCheck = ByteUtils.BytesToUTF8(_decrypted, 6, nameLength);
        EnqueueForSend(Packets.NameCheckResults(Database.CheckIfNameIsUsed(toCheck)), _remote);
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
    public void HandleMatchCreatedPacket(){
        Main.LogMessage("Account " + _accountID + " is creating a match.");
        byte sceneID = _decrypted[5];
        byte duration = _decrypted[6];
        byte matchType = _decrypted[7];
        byte matchOptions = _decrypted[8];
        MatchManager.RequestMatchCreation(_accountID, sceneID, duration, matchType, matchOptions);
    }

    public void HandleMatchSubscribePacket(boolean subscribe){
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        RemoteClient rc = MatchManager.Subscribe(_accountID, subscribe, characterID);
        byte priorMatchID = _decrypted[9];
        if(_decrypted[10] == 1){ // qm
            HandleQMJoin();
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
        byte[] toSend;
        if(validCreds){
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
    private void HandleCharacterDataRequest(){
    }
}
