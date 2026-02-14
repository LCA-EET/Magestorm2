import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class CaptureTheFlag extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Flag> _flags;
    private final ConcurrentHashMap<Byte, Byte> _score;
    private byte[] _flagBytes;
    private final byte[] _currentScores;
    private boolean _flagsChanged;


    public CaptureTheFlag(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte matchOptions) {
        super(matchID, creatorID, creatorName, sceneID, creationTime, duration, MatchType.CaptureTheFlag, matchOptions);
        _flagsChanged = true;
        _currentScores = new byte[3];
        _poolManager = new PoolManager(this);
        _flags = new ConcurrentHashMap<>();
        _score = new ConcurrentHashMap<>();
        for(byte teamID : MatchTeam.TeamCodes_NonNeutral){
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
                        MatchCharacter capturer = _matchCharacters.get(capturedBy);
                        _playerScores.get(capturer.GetCharacterID()).IncrementCapturesFor();
                        _matchTeams.get(capturingTeam).GetScore().IncrementCapturesFor();
                        _matchTeams.get(flagCaptured).GetScore().IncrementCapturesAgainst();
                        RefreshScores();
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
            if(IsCharacterAlive(takenBy) && !IsHoldingFlag(takenBy)){
                taken.FlagTaken(takenBy);
                SendToAll(Packets.FlagTakenPacket(flagTaken, takenBy));
            }
        }
    }
    private void RefreshScores(){
        _currentScores[0] = _matchTeams.get(MatchTeam.Chaos).GetScore().GetCTFScore();
        _currentScores[1] = _matchTeams.get(MatchTeam.Balance).GetScore().GetCTFScore();
        _currentScores[2] = _matchTeams.get(MatchTeam.Order).GetScore().GetCTFScore();
    }
    private void FlagDropped(Flag droppedFlag, MatchCharacter killedPlayer, MatchCharacter killer){
        byte killerID = killer.GetIDinMatch();
        Main.LogMessage("FlagDropped: " + killedPlayer.GetIDinMatch() + ", " + killerID);
        if(droppedFlag.IsHeld()){

            droppedFlag.FlagDropped(killedPlayer.GetPosition());
            _flagsChanged = true;
            SendToAll(Packets.FlagDroppedPacket(killedPlayer.GetIDinMatch(), droppedFlag.GetFlagBytes(), killerID));
        }
    }

    public void FlagReturned(byte returner, byte flag){
        Flag returned = _flags.get(flag);
        if(!returned.IsHeld()){
            if(IsCharacterAlive(returner)){
                returned.FlagReturned();
                _flagsChanged = true;
                SendToAll(Packets.FlagReturnedPacket(flag));
            }
        }
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
        return _currentScores;
    }

    private boolean SeeIfFlagDropped(byte characterID, byte killerID){
        MatchCharacter killed = GetMatchCharacter(characterID);
        MatchCharacter killer = GetMatchCharacter(killerID);
        if(killed != null){
            return SeeIfFlagDropped(killed, killer);
        }
        return false;
    }
    private boolean SeeIfFlagDropped(MatchCharacter character, MatchCharacter killer){
        for(Flag flag : _flags.values()){
            if(flag.GetHolderID() == character.GetIDinMatch()){
                FlagDropped(flag, character, killer);
                return true;
            }
        }
        return false;
    }

    public boolean IsHoldingFlag(byte playerID){
        for(Flag toCheck : _flags.values()){
            if(toCheck.HeldBy() == playerID){
                return true;
            }
        }
        return false;
    }

    private void DropCountdown(long msElapsed){
        for(Flag flag : _flags.values()){
            if(flag.IsDropped()){
                if(flag.DropCountdown(msElapsed)){
                    FlagReturned((byte)0, flag.GetTeamID());
                }
            }
        }
    }
    @Override
    public boolean ParseCommand(String command, String[] params, byte senderID){
        if(!super.ParseCommand(command, params, senderID)){
            switch(command){
                case "dropflag":
                    SeeIfFlagDropped(senderID, senderID);
                    return true;
            }
        }
        return false;
    }

    @Override
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID) {
        MatchCharacter mc = super.JoinMatch(rc, teamID);
        GameServer.EnqueueForSend(Packets.CTFEntryPacket(_sceneID, mc, teamID, _matchPort, _matchID, _matchType), rc);
        return mc;
    }

    @Override
    protected void PlayerKilled(MatchCharacter killed, MatchCharacter killer){
        if(!SeeIfFlagDropped(killed, killer)){
            super.PlayerKilled(killed, killer);
        };
    }

    @Override
    public void LeaveMatch(byte id, boolean send, boolean quitGame){
        Main.LogMessage("Player " + id + " is leaving CTF match.");
        SeeIfFlagDropped(id, (byte)0);
        super.LeaveMatch(id, send, quitGame);
    }

    @Override
    public void Tick(long msElapsed)
    {
        DropCountdown(msElapsed);
        super.Tick(msElapsed);
    }

}
