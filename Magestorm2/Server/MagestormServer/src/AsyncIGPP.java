import java.util.Arrays;

public class AsyncIGPP extends AsyncRequestProcessor{
    private final Match _owningMatch;
    private MatchCharacter _verified;
    public AsyncIGPP(Match owningMatch, InGamePacketProcessor igp)
    {
        super("AsyncIGPP for match " + owningMatch.GetMatchID(), igp);
        _owningMatch = owningMatch;
    }

    @Override
    public void ProcessPacket(AsyncRequest toProcess){
        super.PreProcess(toProcess);
        _verified = ((AsyncIGPPRequest)toProcess).GetVerifiedCharacter();
        switch (toProcess.GetOpCode()) {
            case InGame_Receive.BroadcastMessage ->
                HandleBroadcastMessage();
            case InGame_Receive.TeamMessage ->
                SharedFunctions.HandleTeamMessage(_decrypted, _udp, _owningMatch, _remote, _verified);
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
        if(_owningMatch.IsPlayerVerified(recipientID)){
            int messageLength = ByteUtils.ExtractInt(_decrypted, 3);
            byte[] messageBytes = new byte[messageLength];
            System.arraycopy(_decrypted, 7, messageBytes, 0, messageLength);
            String messageString = ByteUtils.BytesToUTF8(messageBytes);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                _udp.EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage), _remote);
            }
            else{
                Main.LogChat(_verified, messageString, _owningMatch.ObjectID());
                RemoteClient messageRecipient = _owningMatch.GetMatchCharacter(recipientID).GetRemoteClient();
                Iterable<RemoteClient> recipients = Arrays.asList(_remote, messageRecipient);
                _udp.EnqueueForSend(Packets.MessagePacket(_decrypted, messageLength + 7), recipients);
            }
        }
    }
    private void HandleBroadcastMessage(){
        int messageLength = ByteUtils.ExtractInt(_decrypted,2);
        String messageString = ByteUtils.BytesToUTF8(_decrypted, 6, messageLength);
        if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
            _udp.EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage),
                    _remote);
        }
        else{
            Main.LogChat(_verified, messageString, _owningMatch.ObjectID());
            if(messageString.startsWith("/")){
                String[] split = messageString.split(" ");
                _owningMatch.ParseCommand(split[0].toLowerCase().substring(1), split, _verified, _decrypted[1]);
            }
            else{
                _udp.EnqueueForSend(Packets.MessagePacket(_decrypted, 6 + messageLength),
                        _owningMatch.GetVerifiedClients());
            }
        }
    }

}
