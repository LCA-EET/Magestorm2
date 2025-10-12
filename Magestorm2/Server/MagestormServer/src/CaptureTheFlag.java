import java.util.concurrent.ConcurrentHashMap;

public class CaptureTheFlag extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Flag> _flags;
    private final ConcurrentHashMap<Byte, Byte> _score;

    public CaptureTheFlag(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.CaptureTheFlag);
        _poolManager = new PoolManager(this);
        _flags = new ConcurrentHashMap<>();
        _score = new ConcurrentHashMap<>();
        for(byte teamID : MatchTeam.TeamCodes){
            _flags.put(teamID, new Flag(teamID));
            _score.put(teamID, (byte)0);
        }
        _processor = new InGamePacketProcessor(_matchPort, this);
    }

    public PoolManager GetPoolManager(){
        return _poolManager;
    }

    public void FlagCaptured(byte capturedBy, byte flagCaptured){
        byte capturingTeam = _matchCharacters.get(capturedBy).GetTeamID();
        if(!_flags.get(capturingTeam).IsHeld()){
            if(_flags.get(flagCaptured).IsHeld()){
                AdjustScore(capturingTeam, (byte)1);
                AdjustScore(flagCaptured, (byte)-1);
                _flags.get(flagCaptured).FlagCaptured();
                SendToAll(Packets.FlagCapturedPacket(capturingTeam, flagCaptured, capturedBy,
                        _score.get(capturingTeam), _score.get(flagCaptured)));
            }
        }
    }

    public void FlagReturned(byte flag, byte returner){

    }
    private void AdjustScore(byte team, byte adjustment){
        byte currentScore = _score.get(team);
        currentScore += adjustment;
        _score.put(team, currentScore);
    }
}
