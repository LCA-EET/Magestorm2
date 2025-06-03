import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentLinkedQueue;

public class PregamePacketProcessor implements PacketProcessor
{
    private UDPClient _udpClient;
    private PacketSender _sender;
    private int _serverPort;
    private ConcurrentLinkedQueue<OutgoingPacket> _outgoingPackets;

    public PregamePacketProcessor(){
        _serverPort = ServerParams.ListeningPort;
        _outgoingPackets = new ConcurrentLinkedQueue<>();
        _udpClient = new UDPClient(_serverPort, this);
        _sender = new PacketSender(_udpClient, this);
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {
        RemoteClient rc = new RemoteClient(received, _serverPort);
        byte[] receivedBytes = received.getData();
        byte[] decrypted = Cryptographer.Decrypt(receivedBytes);
        byte opCode = decrypted[0];
        Main.LogMessage("OpCode: " + opCode);
        switch (opCode) {
            case OpCode_Receive.LogIn:
                HandleLogInPacket(decrypted, rc);
                break;
            case OpCode_Receive.CreateAccount:
                HandleCreateAccountPacket(decrypted, rc);
                break;
            case OpCode_Receive.CreateCharacter:
                HandleCreateCharacterPacket(decrypted, rc);
                break;
            case OpCode_Receive.LogOut:
                HandleLogOutPacket(decrypted);
                break;
            case OpCode_Receive.DeleteCharacter:
                HandleDeleteCharacterPacket(decrypted, rc);
                break;
            case OpCode_Receive.SubscribeToMatches:
                HandleMatchSubscribePacket(decrypted, true);
                break;
            case OpCode_Receive.UnsubscribeFromMatches:
                HandleMatchSubscribePacket(decrypted, false);
                break;
            case OpCode_Receive.CreateMatch:
                HandleMatchCreatedPacket(decrypted, rc);
                break;
            case OpCode_Receive.DeleteMatch:
                HandleDeleteMatchPacket(decrypted, rc);
                break;
            case OpCode_Receive.RequestLevelsList:
                HandleLevelListPacket(decrypted, rc);
                break;
            case OpCode_Receive.RequestMatchDetails:
                HandleMatchDetailsPacket(decrypted, rc);
                break;
            case OpCode_Receive.NameCheck:
                HandleNameCheckPacket(decrypted, rc);
                break;
            case OpCode_Receive.UpdateAppearance:
                HandleAppearanceUpdatePacket(decrypted);
                break;
            case OpCode_Receive.JoinMatch:
                HandleJoinMatchPacket(decrypted);
                break;
        }
    }
    private int IsLoggedIn(byte[] decrypted){
        int accountID = ByteUtils.ExtractInt(decrypted, 1);
        return GameServer.IsLoggedIn(accountID) ? accountID: 0;
    }
    private void HandleJoinMatchPacket(byte[] decrypted)
    {
        int accountID = IsLoggedIn(decrypted);
        if(accountID > 0){
            byte matchID = decrypted[5];
            byte teamID = decrypted[6];
            Match toJoin = MatchManager.GetMatch(matchID);
            if(toJoin != null){
                toJoin.JoinMatch(GameServer.GetClient(accountID), teamID);

            }
        }
    }

    public void HandleAppearanceUpdatePacket(byte[] decrypted){
        if(IsLoggedIn(decrypted) > 0){
            int characterID = ByteUtils.ExtractInt(decrypted, 5);
            byte[] appearanceBytes = new byte[5];
            System.arraycopy(decrypted, 9, appearanceBytes, 0, appearanceBytes.length);
            Database.UpdateCharacterAppearance(characterID, appearanceBytes);
        }
    }
    public void HandleNameCheckPacket(byte[] decrypted, RemoteClient rc){
        if(IsLoggedIn(decrypted) > 0){
            byte nameLength = decrypted[5];
            String toCheck = ByteUtils.BytesToUTF8(decrypted, 6, nameLength);
            EnqueueForSend(Packets.NameCheckResults(Database.CheckIfNameIsUsed(toCheck)), rc);
        }
    }
    public void HandleMatchDetailsPacket(byte[] decrypted, RemoteClient rc){
        if(IsLoggedIn(decrypted) > 0){
            byte matchID = decrypted[5];
            Match match = MatchManager.GetMatch(matchID);
            if(match != null){
                EnqueueForSend(Packets.MatchDetailsPacket(match), rc);
            }
        }
    }
    public void HandleLevelListPacket(byte[] decrypted, RemoteClient rc){
        if(IsLoggedIn(decrypted) > 0){
            EnqueueForSend(Packets.LevelListPacket(), rc);
        }
    }
    public void HandleDeleteMatchPacket(byte[] decrypted, RemoteClient rc){
        int accountID = IsLoggedIn(decrypted);
        if(accountID > 0){
            MatchManager.DeleteMatch(accountID, rc);
        }
    }
    public void HandleMatchCreatedPacket(byte[] decrypted, RemoteClient rc){
        int accountID = IsLoggedIn(decrypted);
        if(accountID > 0){
            byte sceneID = decrypted[5];
            MatchManager.RequestMatchCreation(accountID, sceneID);
        }
    }

    public void HandleMatchSubscribePacket(byte[] decrypted, boolean subscribe){
        int accountID = ByteUtils.ExtractInt(decrypted,1);
        int characterID = ByteUtils.ExtractInt(decrypted, 5);
        MatchManager.Subscribe(accountID, subscribe, characterID);
    }

    public String[] LogInDetails(byte[] decrypted){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(decrypted, 3);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        String[] toReturn = new String[2];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        return toReturn;
    }

    private void HandleDeleteCharacterPacket(byte[] decrypted, RemoteClient rc){
        int accountID = IsLoggedIn(decrypted);
        if(accountID > 0){
            int characterID = ByteUtils.ExtractInt(decrypted, 5);
            Main.LogMessage("Deactivating character: " + characterID);
            Database.DeactivateCharacter(characterID, accountID);
            EnqueueForSend(Packets.CharacterDeletedPacket(characterID), rc);
        }
    }

    private void HandleLogOutPacket(byte[] decrypted){
        int accountID = IsLoggedIn(decrypted);
        if(accountID > 0){
            GameServer.ClientLoggedOut(accountID);
        }
    }

    private void HandleCreateCharacterPacket(byte[] decrypted, RemoteClient rc){
        int accountID = IsLoggedIn(decrypted);
        if(accountID > 0){
            byte classCode = decrypted[5];
            byte[] stats = new byte[6];
            byte[] appearance = new byte[5];
            System.arraycopy(decrypted, 6, stats, 0, 6);
            System.arraycopy(decrypted, 12, appearance, 0, 5);
            if(AntiCheat.CheckStats(stats, rc, accountID)){
                return;
            }
            byte nameLength = decrypted[17];
            String characterName = new String(Packets.ExtractBytes(decrypted, 18, nameLength),
                    StandardCharsets.UTF_8);
            if(ProfanityChecker.ContainsProhibitedLanguage(characterName)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(), rc);
            }
            else{
                if(Database.SeeIfCharacterExists(characterName)){
                    EnqueueForSend(Packets.CharacterExistsPacket(), rc);
                }
                else{
                    int charID = Database.AddCharacter(accountID, characterName, classCode, stats, appearance);
                    if(charID == -1){
                        EnqueueForSend(Packets.CreationFailedPacket(), rc);
                    }
                    else{
                        EnqueueForSend(Packets.CharacterCreatedPacket(charID, classCode, characterName, stats, appearance), rc);
                    }
                }
            }
        }
    }

    private void HandleLogInPacket(byte[] decrypted, RemoteClient rc){
        String[] creds = LogInDetails(decrypted);
        String username = creds[0];
        String hashed = creds[1];
        Object[] validationResult = Database.ValidateCredentials(username, hashed);
        boolean validCreds = (boolean)validationResult[0];
        int accountID = (int)validationResult[1];
        byte[] toSend;
        if(validCreds){
            if(GameServer.IsLoggedIn(accountID)){
                toSend = Packets.AlreadyLoggedInPacket();
                RemoteClient alreadyExisting = GameServer.RemoveClient(accountID);
                if(alreadyExisting != null){
                    EnqueueForSend(Packets.RemovedFromServerPacket(RemovalReason.AlreadyLoggedIn),
                            alreadyExisting);
                }
            }
            else {
                toSend = Packets.LoginSucceededPacket(accountID);
                rc.SetNameAndID(username, accountID);
                GameServer.ClientLoggedIn(rc);
            }
        }
        else{
            toSend = Packets.LoginFailedPacket();
        }
        EnqueueForSend(toSend,rc);
    }

    public String[] CreateAccountDetails(byte[] decrypted){
        ArrayList<byte[]> toProcess = Packets.ExtractBytes(decrypted, 4);
        byte[] userNameBytes = toProcess.get(0);
        byte[] pwHashBytes = toProcess.get(1);
        byte[] emailBytes = toProcess.get(2);
        String[] toReturn = new String[3];
        toReturn[0] = new String(userNameBytes, StandardCharsets.UTF_8);
        toReturn[1] = Base64.getEncoder().encodeToString(pwHashBytes);
        toReturn[2] = new String(emailBytes, StandardCharsets.UTF_8);
        return toReturn;
    }
    private void HandleCreateAccountPacket(byte[] decrypted, RemoteClient rc){
        String[] creds = CreateAccountDetails(decrypted);
        String username = creds[0];
        if(!ProfanityChecker.ContainsProhibitedLanguage(username)){
            String email = creds[2];
            Main.LogMessage("Account creation requested: " + username + ", " + email);
            if(Database.AccountRecordCount(username, email) > 0){
                EnqueueForSend(Packets.AccountExistsPacket(), rc);
                Main.LogMessage("Account " + username + " already exists .");
            }
            else{
                Main.LogMessage("Account " + username + " does not already exist.");
                long token = Cryptographer.RandomToken();
                boolean accountCreated = Database.CreateAccount(username, creds[1], email, token);
                byte[] toSend = accountCreated?Packets.AccountCreatedPacket():Packets.CreationFailedPacket();
                EnqueueForSend(toSend, rc);
                String activationMessage = "Hello<br><br>Click the following link to activate your Magus account:<br><a href='https://www.fosiemods.net/ms2.php?appid=ms2&func=activate&activationtoken=" + token + "'>Activation Link</a>";
                Main.Mailer.SendMail(email, "Magus Account Activation Link", activationMessage, "Magus Activation");
            }
        }
        else{
            EnqueueForSend(Packets.ProhibitedLanguagePacket(), rc);
        }

    }
    public void EnqueueForSend(byte[] data, RemoteClient rc){
        _outgoingPackets.add(new OutgoingPacket(data, rc));
    }
    public void EnqueueForSend(byte[] data, RemoteClient[] rc){
        _outgoingPackets.add(new OutgoingPacket(data, rc));
    }

    @Override
    public ArrayList<OutgoingPacket> OutgoingPackets() {
        ArrayList<OutgoingPacket> toReturn = new ArrayList<>();
        while(!_outgoingPackets.isEmpty()){
            toReturn.add(_outgoingPackets.remove());
        }
        return toReturn;
    }

    @Override
    public boolean HasOutgoingPackets(){
        return !_outgoingPackets.isEmpty();
    }
}
