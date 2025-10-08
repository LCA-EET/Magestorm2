public class DeathMatch extends Match{
    private final PoolManager _poolManager;

    public DeathMatch(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.DeathMatch);
        _poolManager = new PoolManager(this);
        _processor = new DMPacketProcessor(_matchPort, this);
    }

    public PoolManager GetPoolManager(){
        return _poolManager;
    }

    private byte ShrineHealth(byte teamID){
        return _matchTeams.get(teamID).ShrineHealth();
    }
    public byte[] ReportAllShrineHealth(){
        return new byte[]{
                ShrineHealth(MatchTeam.Chaos) ,
                ShrineHealth(MatchTeam.Balance),
                ShrineHealth(MatchTeam.Order)
        };
    }

    @Override
    public byte JoinMatch(RemoteClient rc, byte teamID) {
        byte playerID = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.DeathMatchEntryPacket(_sceneID, teamID, playerID, _matchPort, _matchID, MatchType.DeathMatch), rc);
        return playerID;
    }
}
