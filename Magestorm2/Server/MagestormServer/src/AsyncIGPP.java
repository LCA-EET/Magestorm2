import java.util.Arrays;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

public class AsyncIGPP extends RegisteredThread{
    private final BlockingQueue<AsyncIGPPRequest> _toProcess;
    private final Match _owningMatch;
    private final InGamePacketProcessor _igp;
    private RemoteClient _remote;
    private MatchCharacter _verified;
    private byte[] _decrypted;
    private boolean _terminated;
    public AsyncIGPP(Match owningMatch, InGamePacketProcessor igp)
    {
        _owningMatch = owningMatch;
        _toProcess = new LinkedBlockingQueue<>();
        _igp = igp;
    }

    public void EnqueueRequest(AsyncIGPPRequest toEnqueue){
        _toProcess.add(toEnqueue);
    }

    public void ProcessPacket(AsyncIGPPRequest toProcess){
        _verified = toProcess.GetVerifiedCharacter();
        _decrypted = toProcess.GetBytes();
        _remote = toProcess.GetSender();
        switch (toProcess.GetOpCode()) {
            case InGame_Receive.BroadcastMessage ->
                HandleBroadcastMessage();
            case InGame_Receive.TeamMessage ->
                SharedFunctions.HandleTeamMessage(_decrypted, _igp, _owningMatch, _remote, _verified);
            case InGame_Receive.DirectMessage ->
                HandleDirectMessage();
            case InGame_Receive.RequestWallsAndSigils ->
                _owningMatch.HandleWallAndSigilRequest(_verified);
            case InGame_Receive.LeaderboardRequest ->
                HandleLeaderboardRequest();
            case InGame_Receive.AllPlayerData ->
                _owningMatch.SendAllPlayerData(_verified);
        };
    }
    private void HandleLeaderboardRequest(){
        byte[] toEncrypt = MatchManager.GetScoreBytes((byte)_owningMatch.ObjectID());
        _owningMatch.SendToPlayer(Packets.MatchScoresPacket(InGame_Send.MatchScores, toEncrypt == null ? new byte[2] : toEncrypt),
                _verified);
    }
    private void HandleDirectMessage(){
        byte recipientID = _decrypted[2];
        if(_igp.IsVerified(recipientID)){
            int messageLength = ByteUtils.ExtractInt(_decrypted, 3);
            byte[] messageBytes = new byte[messageLength];
            System.arraycopy(_decrypted, 7, messageBytes, 0, messageLength);
            String messageString = ByteUtils.BytesToUTF8(messageBytes);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                _igp.EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage), _remote);
            }
            else{
                Main.LogChat(_verified, messageString, _owningMatch.ObjectID());
                RemoteClient messageRecipient = _owningMatch.GetMatchCharacter(recipientID).GetRemoteClient();
                Iterable<RemoteClient> recipients = Arrays.asList(_remote, messageRecipient);
                _igp.EnqueueForSend(Packets.MessagePacket(_decrypted, messageLength + 7), recipients);
            }
        }
    }
    private void HandleBroadcastMessage(){
        int messageLength = ByteUtils.ExtractInt(_decrypted,2);
        String messageString = ByteUtils.BytesToUTF8(_decrypted, 6, messageLength);
        if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
            _igp.EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage),
                    _remote);
        }
        else{
            Main.LogChat(_verified, messageString, _owningMatch.ObjectID());
            if(messageString.startsWith("/")){
                String[] split = messageString.split(" ");
                _owningMatch.ParseCommand(split[0].toLowerCase().substring(1), split, _verified, _decrypted[1]);
            }
            else{
                _igp.EnqueueForSend(Packets.MessagePacket(_decrypted, 6 + messageLength),
                        _owningMatch.GetVerifiedClients());
            }
        }
    }
    @Override
    public void run(){
        Register("AsyncPacketProcessor for match " + _owningMatch.GetMatchID());
        while(!_terminated){
            try{
                ProcessPacket(_toProcess.take());
            }
            catch(InterruptedException ex){
                _terminated = true;
            }
            catch(Exception ex){
                Main.LogError(ex.getMessage());
                Main.LogStackTrace(ex);
            }
        }
        Deregister();
    }
}
