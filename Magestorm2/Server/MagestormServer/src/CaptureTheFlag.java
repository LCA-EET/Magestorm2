import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class CaptureTheFlag extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Flag> _flags;
    private final ConcurrentHashMap<Byte, Byte> _score;
    private byte[] _flagBytes;
    private final byte[] _scores;
    private boolean _flagsChanged;

    public CaptureTheFlag(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.CaptureTheFlag);
        _flagsChanged = true;
        _poolManager = new PoolManager(this);
        _flags = new ConcurrentHashMap<>();
        _score = new ConcurrentHashMap<>();
        _scores = new byte[3];
        for(byte teamID : MatchTeam.TeamCodes){
            _flags.put(teamID, new Flag(teamID));
            _score.put(teamID, (byte)0);
        }
        _processor = new CTFPacketProcessor(_matchPort, this);
    }

    public PoolManager GetPoolManager(){
        return _poolManager;
    }

    public void FlagCaptured(byte capturedBy, byte flagCaptured){
        byte capturingTeam = _matchCharacters.get(capturedBy).GetTeamID();
        if(!_flags.get(capturingTeam).IsHeld()){
            Flag captured = _flags.get(flagCaptured);
            if(captured != null){
                if(captured.IsHeld()){
                    if(IsCharacterAlive(capturedBy)){
                        AdjustScore(capturingTeam, (byte)1);
                        AdjustScore(flagCaptured, (byte)-1);
                        captured.FlagReturned();
                        _flagsChanged = true;
                        SendToAll(Packets.FlagCapturedPacket(capturingTeam, flagCaptured, capturedBy,
                                _score.get(capturingTeam), _score.get(flagCaptured)));
                    }
                }
            }
        }
    }

    public void FlagTaken(byte flagTaken, byte takenBy){
        Flag taken = _flags.get(flagTaken);
        if(!taken.IsHeld()){
            if(IsCharacterAlive(takenBy)){
                SendToAll(Packets.FlagTakenPacket(flagTaken, takenBy));
            }
        }
    }

    public void FlagDropped(byte[] decrypted){
        byte flagID = decrypted[1];
        Flag dropped = _flags.get(flagID);
        if(dropped.IsHeld()){
            byte holderID = decrypted[2];
            byte[] positionBytes = new byte[12];
            System.arraycopy(decrypted, 3, positionBytes, 0, positionBytes.length);
            dropped.FlagDropped(positionBytes);
            _flagsChanged = true;
            SendToAll(Packets.FlagDroppedPacket(holderID, dropped.GetFlagBytes()));
        }
    }

    public void FlagReturned(byte returner, byte flag){
        Flag returned = _flags.get(flag);
        if(returned.IsHeld()){
            if(IsCharacterAlive(returner)){
                returned.FlagReturned();
                _flagsChanged = true;
                SendToAll(Packets.FlagReturnedPacket(flag, returner));
            }
        }
    }
    private void AdjustScore(byte team, byte adjustment){
        byte currentScore = _score.get(team);
        currentScore += adjustment;
        _score.put(team, currentScore);
        _scores[team-1] = currentScore;
    }
    public byte[] FlagsStatus(){
        if(_flagsChanged){
            int length = 0;
            ArrayList<byte[]> holder = new ArrayList<>();
            for(Flag flag : _flags.values()){
                byte[] flagBytes = flag.GetFlagBytes();
                if(flagBytes[1] == Flag.DROPPED){
                    holder.add(flagBytes);
                    length += flagBytes.length;
                }
                else{
                    holder.add(new byte[]{flagBytes[0], flagBytes[1]});
                    length+=2;
                }
            }
            _flagBytes = new byte[length];
            int index = 0;
            for(byte[] bytes : holder){
                System.arraycopy(bytes, 0, _flagBytes, index, bytes.length);
                index += bytes.length;
            }
            _flagsChanged = false;
        }
        return _flagBytes;
    }

    public byte[] GetScores(){
        return _scores;
    }

    @Override
    public byte JoinMatch(RemoteClient rc, byte teamID) {
        byte playerID = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.CTFEntryPacket(_sceneID, playerID, teamID, _matchPort, _matchID, _matchType), rc);
        return playerID;
    }
}
