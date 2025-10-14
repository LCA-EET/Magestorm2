public class FreeForAll extends Match{
    public FreeForAll(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.FreeForAll);
        _processor = new InGamePacketProcessor(_matchPort, this);
    }

    @Override
    public byte JoinMatch(RemoteClient rc, byte teamID) {
        Main.LogMessage("Joining FFA " + _matchID + ", scene: " + _sceneID);
        byte playerID = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.FFAEntryPacket(_sceneID, playerID, _matchPort, _matchType), rc);
        return playerID;
    }
}
