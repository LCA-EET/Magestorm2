import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.ArrayList;

public class PregamePacketProcessor extends UDPProcessor
{

    public PregamePacketProcessor(int port){
        super(port);
    }

    @Override
    protected void ProcessPacket(DatagramPacket received) {
        PreProcess(received);
        switch (_opCode) {
            case Pregame_Receive.LogIn:
                HandleLogInPacket();
                break;
            case Pregame_Receive.CreateAccount:
                HandleCreateAccountPacket();
                break;
            case Pregame_Receive.CreateCharacter:
                HandleCreateCharacterPacket();
                break;
            case Pregame_Receive.LogOut:
                HandleLogOutPacket();
                break;
            case Pregame_Receive.DeleteCharacter:
                HandleDeleteCharacterPacket();
                break;
            case Pregame_Receive.SubscribeToMatches:
                HandleMatchSubscribePacket(true);
                break;
            case Pregame_Receive.UnsubscribeFromMatches:
                HandleMatchSubscribePacket(false);
                break;
            case Pregame_Receive.CreateMatch:
                HandleMatchCreatedPacket();
                break;
            case Pregame_Receive.DeleteMatch:
                HandleDeleteMatchPacket();
                break;
            case Pregame_Receive.RequestLevelsList:
                HandleLevelListPacket();
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
                HandleMatchListRequest();
                break;
        }
    }
    private void HandleMatchListRequest(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            MatchManager.SendMatchListToClient(_remote);
        }
    }
    private void HandleJoinMatchPacket()
    {
        int accountID = IsLoggedIn();
        if(accountID > 0){
            byte matchID = _decrypted[5];
            byte teamID = _decrypted[6];
            Match toJoin = MatchManager.GetMatch(matchID);
            if(toJoin != null){
                if(toJoin.HasRoomForAnotherPlayer()){
                    RemoteClient remote = GameServer.GetClient(accountID);
                    remote.UnsubscribeFromMatches();
                    toJoin.JoinMatch(remote, teamID);
                }
                else{
                    EnqueueForSend(Packets.MatchIsFullPacket(), _remote);
                }
            }
        }
    }

    public void HandleAppearanceUpdatePacket(){
        if(IsLoggedIn() > 0){
            int characterID = ByteUtils.ExtractInt(_decrypted, 5);
            byte[] appearanceBytes = new byte[5];
            System.arraycopy(_decrypted, 9, appearanceBytes, 0, appearanceBytes.length);
            Database.UpdateCharacterAppearance(characterID, appearanceBytes);
        }
    }
    public void HandleNameCheckPacket(){
        if(IsLoggedIn() > 0){
            byte nameLength = _decrypted[5];
            String toCheck = ByteUtils.BytesToUTF8(_decrypted, 6, nameLength);
            EnqueueForSend(Packets.NameCheckResults(Database.CheckIfNameIsUsed(toCheck)), _remote);
        }
    }
    public void HandleMatchDetailsPacket(){
        if(IsLoggedIn() > 0){
            byte matchID = _decrypted[5];
            Match match = MatchManager.GetMatch(matchID);
            if(match != null){
                EnqueueForSend(Packets.MatchDetailsPacket(match), _remote);
            }
        }
    }
    public void HandleLevelListPacket(){
        if(IsLoggedIn() > 0){
            EnqueueForSend(Packets.LevelListPacket(), _remote);
        }
    }
    public void HandleDeleteMatchPacket(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            MatchManager.DeleteMatch(accountID, _remote);
        }
    }
    public void HandleMatchCreatedPacket(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            Main.LogMessage("Account " + accountID + " is creating a match.");
            byte sceneID = _decrypted[5];
            byte duration = _decrypted[6];
            byte matchType = _decrypted[7];
            MatchManager.RequestMatchCreation(accountID, sceneID, duration, matchType);
        }
    }

    public void HandleMatchSubscribePacket(boolean subscribe){
        int accountID = ByteUtils.ExtractInt(_decrypted,1);
        int characterID = ByteUtils.ExtractInt(_decrypted, 5);
        MatchManager.Subscribe(accountID, subscribe, characterID);
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
        int accountID = IsLoggedIn();
        if(accountID > 0){
            int characterID = ByteUtils.ExtractInt(_decrypted, 5);
            Main.LogMessage("Deactivating character: " + characterID);
            Database.DeactivateCharacter(characterID, accountID);
            EnqueueForSend(Packets.CharacterDeletedPacket(characterID), _remote);
        }
    }

    private void HandleLogOutPacket(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            GameServer.ClientLoggedOut(accountID);
        }
    }

    private void HandleCreateCharacterPacket(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            byte classCode = _decrypted[5];
            byte[] stats = new byte[6];
            byte[] appearance = new byte[5];
            System.arraycopy(_decrypted, 6, stats, 0, 6);
            System.arraycopy(_decrypted, 12, appearance, 0, 5);
            if(AntiCheat.CheckStats(stats, _remote, accountID)){
                return;
            }
            byte nameLength = _decrypted[17];
            String characterName = new String(Packets.ExtractBytes(_decrypted, 18, nameLength),
                    StandardCharsets.UTF_8);
            if(ProfanityChecker.ContainsProhibitedLanguage(characterName)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(Pregame_Send.ProhibitedLanguage), _remote);
            }
            else{
                if(Database.SeeIfCharacterExists(characterName)){
                    EnqueueForSend(Packets.CharacterExistsPacket(), _remote);
                }
                else{
                    int charID = Database.AddCharacter(accountID, characterName, classCode, stats, appearance);
                    if(charID == -1){
                        EnqueueForSend(Packets.CreationFailedPacket(), _remote);
                    }
                    else{
                        EnqueueForSend(Packets.CharacterCreatedPacket(charID, classCode, characterName, stats, appearance), _remote);
                    }
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
