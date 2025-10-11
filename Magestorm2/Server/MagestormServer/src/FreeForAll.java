public class FreeForAll extends Match{
    public FreeForAll(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.DeathMatch);
        _processor = new InGamePacketProcessor(_matchPort, this);
    }
}
