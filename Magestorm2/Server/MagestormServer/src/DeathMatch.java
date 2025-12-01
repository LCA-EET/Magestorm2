import java.util.concurrent.ConcurrentHashMap;

public class DeathMatch extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Shrine> _shrines;

    public DeathMatch(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte[] matchOptions) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.DeathMatch, matchOptions);
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
    public void SendToTeam(byte[] encrypted, byte teamID){
        SendToCollection(encrypted, _matchTeams.get(teamID).GetRemoteClients());
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

    public boolean IsTeamAlive(byte teamID){
        Shrine toCheck = _shrines.get(teamID);
        if(toCheck.ShrineHealth() > 0){
            return true;
        }
        else{
            MatchTeam team = _matchTeams.get(teamID);
            for(MatchCharacter mc: team.GetPlayers()){
                if(mc.IsAlive()){
                    return true;
                }
            }
            return false;
        }
    }

    @Override
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID) {
        MatchCharacter mc = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.DeathMatchEntryPacket(_sceneID, teamID, mc, _matchPort, _matchID, _matchType), rc);
        return mc;
    }

    @Override
    public void PlayerTapped(byte playerID){
        MatchCharacter mc = _matchCharacters.get(playerID);
        byte teamID = mc.GetTeamID();
        LogMessage("DM tap: " + playerID + "." );
        if(teamID == MatchTeam.Neutral){
            super.PlayerTapped(playerID);
        }
        else{
            if(_shrines.get(teamID).IsAlive()){
                super.PlayerTapped(playerID);
            }
        }
    }
}
