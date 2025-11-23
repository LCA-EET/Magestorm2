public class FreeForAll extends Match{
    public FreeForAll(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte[] matchOptions) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.FreeForAll, matchOptions);
        Main.LogMessage("Initializing FFA");
        _processor = new InGamePacketProcessor(_matchPort, this);
    }

    @Override
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID) {
        Main.LogMessage("Joining FFA " + _matchID + ", scene: " + _sceneID);
        MatchCharacter mc = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.FFAEntryPacket(_sceneID, mc, _matchPort, _matchType, _matchID), rc);
        return mc;
    }
}
