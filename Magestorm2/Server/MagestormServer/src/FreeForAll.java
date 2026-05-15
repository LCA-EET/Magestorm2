public class FreeForAll extends Match{

    protected FreeForAll(byte matchID, int creatorID, byte sceneID, byte duration, byte matchOptions){
        this(matchID, creatorID, ByteUtils.UTF8toBytes("QuickMatch"), sceneID, duration,  matchOptions);
        _quickMatch = true;
        MatchManager.SetQMID(matchID);
    }

    public FreeForAll(byte matchID, int creatorID, byte[] creatorName, byte sceneID,  byte duration, byte matchOptions) {
        super(matchID, creatorID, creatorName, sceneID, duration, MatchType.FreeForAll, matchOptions);
        Main.LogMessage("Initializing FFA");
        _processor = new InGamePacketProcessor(_matchPort, this);
    }

    @Override
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID) {
        Main.LogMessage("Joining FFA " + _matchID + ", scene: " + _sceneID);
        MatchCharacter mc = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.FFAEntryPacket(_sceneID, mc, _matchPort, _matchType, _matchID, _expirationTime), rc);
        return mc;
    }
}
