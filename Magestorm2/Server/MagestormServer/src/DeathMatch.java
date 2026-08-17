import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class DeathMatch extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Shrine> _shrines;
    private long _victoryCountdownRemaining = GameServer.VictoryCountdown;
    private byte _matchWinner;
    private boolean _victoryCountdown = false;

    public DeathMatch(byte matchID, int creatorID, byte[] creatorName, byte sceneID,  byte duration, byte matchOptions) {
        super(matchID, creatorID, creatorName, sceneID, duration, MatchType.DeathMatch, matchOptions);
        _poolManager = new PoolManager(this);
        _shrines = new ConcurrentHashMap<>();
        for(int i = 1; i < MatchTeam.TeamCodes.length; i++){
            _shrines.put(MatchTeam.TeamCodes[i], new Shrine(MatchTeam.TeamCodes[i], this));
        }
        _processor = new DMPacketProcessor(_matchPort, this);
    }
    public void ResetVictoryCountdown(){
        _victoryCountdown = false;
        _victoryCountdownRemaining = GameServer.VictoryCountdown;
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

    public void CheckVictoryCondition(){
        byte fullPower = 0;
        byte destroyed = 0;
        for(Shrine shrine : _shrines.values()){
            if(shrine.ShrineHealth() == 100){
                fullPower++;
                _matchWinner = shrine.GetShrineTeam();
            }
            else if (shrine.ShrineHealth() == 0){
                destroyed++;
            }
        }
        if(fullPower == 1 && (destroyed == _shrines.size() - 1)){
            _victoryCountdown = true;
            LogMessage("Victory condition triggered - winning team: " + _matchWinner);
        }
        else{
            LogMessage("Victory condition cancelled.");
            ResetVictoryCountdown();
        }
    }

    private void VictoryCountdown(long msElapsed){
        _victoryCountdownRemaining -= msElapsed;
        if(_victoryCountdownRemaining <= 0){
            _durationRemaining = 0;
        }
    }


    @Override
    protected void MatchEndedNotification(ArrayList<RemoteClient> remainingClients){
        SendToCollection(Packets.MatchEndedPacket(_matchWinner), remainingClients);
    }
    @Override
    public void Tick(long msElapsed){
        if(_victoryCountdown){
            VictoryCountdown(msElapsed);
        }
        super.Tick(msElapsed);
    }


    @Override
    public boolean ParseCommand(String command, String[] params, byte senderID) {
        if(!super.ParseCommand(command, params, senderID)){
            switch(command){
                case "killshrine":
                    _shrines.get(Byte.parseByte(params[1])).SetShrineHealth((byte)0, senderID);
                    return true;
                case "restoreshrine":
                    _shrines.get(Byte.parseByte(params[1])).SetShrineHealth((byte)100, senderID);
                    return true;
                case "killothershrines":
                    MatchCharacter sender = _matchCharacters.get(senderID);
                    for(Shrine shrine : _shrines.values()){
                        if(shrine.GetShrineTeam() != sender.GetTeamID()){
                            shrine.SetShrineHealth((byte)0, senderID);
                        }
                    }
                    return true;
            }
        }
        return false;
    }
    @Override
    public boolean JoinAlive(byte teamID){
        return _shrines.get(teamID).IsAlive();
    }
    @Override
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID) {
        MatchCharacter mc = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.DeathMatchEntryPacket(_sceneID, teamID, mc, _matchPort, _objectIDAsByte, _matchType, _expirationTime), rc);
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
