import java.util.concurrent.ConcurrentHashMap;

public class DeathMatch extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Shrine> _shrines;

    public DeathMatch(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.DeathMatch);
        _poolManager = new PoolManager(this);
        _shrines = new ConcurrentHashMap<>();
        for(int i = 1; i < MatchTeam.TeamCodes.length; i++){
            _shrines.put(MatchTeam.TeamCodes[i], new Shrine(MatchTeam.TeamCodes[i], this));
        }
        _processor = new DMPacketProcessor(_matchPort, this);
    }

    public PoolManager GetPoolManager(){
        return _poolManager;
    }

    private byte ShrineHealth(byte teamID){
        return _shrines.get(teamID).ShrineHealth();
    }
    public byte[] ReportAllShrineHealth(){
        return new byte[]{
                ShrineHealth(MatchTeam.Chaos) ,
                ShrineHealth(MatchTeam.Balance),
                ShrineHealth(MatchTeam.Order)
        };
    }

    public void AdjustShrineHealth(byte adjusterID,byte shrineID){
        if(_shrines.containsKey(shrineID)){
            MatchCharacter adjuster = GetMatchCharacter(adjusterID);
            if(adjuster.IsAlive()){
                short diceRoll = GameUtils.DiceRoll(100, 1);
                if(Shrine.AdjustmentChance(adjuster.GetClassCode()) >= diceRoll){
                    _shrines.get(shrineID).AdjustShrineHealth(adjuster);
                }
                else{
                    SendToPlayer(Packets.ShrineFailurePacket(shrineID), adjuster);
                }
            }
        }
    }

    @Override
    public byte JoinMatch(RemoteClient rc, byte teamID) {
        byte playerID = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.DeathMatchEntryPacket(_sceneID, teamID, playerID, _matchPort, _matchID, MatchType.DeathMatch), rc);
        return playerID;
    }
}
