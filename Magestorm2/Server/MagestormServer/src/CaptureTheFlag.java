import java.sql.Ref;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class CaptureTheFlag extends Match{
    private final PoolManager _poolManager;
    private final ConcurrentHashMap<Byte, Flag> _flags;
    private byte[] _flagBytes;
    private final byte[] _currentScores;
    private boolean _flagsChanged;


    public CaptureTheFlag(byte matchID, int creatorID, byte[] creatorName, byte sceneID, byte duration, byte matchOptions) {
        super(matchID, creatorID, creatorName, sceneID, duration, MatchType.CaptureTheFlag, matchOptions);
        _flagsChanged = true;
        _currentScores = new byte[3];
        _poolManager = new PoolManager(this);
        _flags = new ConcurrentHashMap<>();
        for(byte teamID : MatchTeam.TeamCodes_NonNeutral){
            _flags.put(teamID, new Flag(teamID));
        }
        _processor = new CTFPacketProcessor(_matchPort, this);
    }

    public PoolManager GetPoolManager(){
        return _poolManager;
    }

    public void FlagCaptured(MatchCharacter verified, byte flagCaptured){
        if(verified.IsAlive()){
            byte capturingTeam = verified.GetTeamID();
            if(!_flags.get(capturingTeam).IsHeld()){
                Flag captured = _flags.get(flagCaptured);
                if(captured != null){
                    if(captured.IsHeld()){
                        GetPlayerScore(verified).IncrementCapturesFor();
                        _matchTeams.get(capturingTeam).GetScore().IncrementCapturesFor();
                        _matchTeams.get(flagCaptured).GetScore().IncrementCapturesAgainst();
                        captured.FlagReturned();
                        RefreshScores();
                        _flagsChanged = true;
                        SendToAll(Packets.FlagCapturedPacket(capturingTeam, flagCaptured, verified.GetIDinMatch(),
                                _matchTeams.get(capturingTeam).GetScore().GetCTFScore(),
                                _matchTeams.get(flagCaptured).GetScore().GetCTFScore()));
                    }
                }
            }
        }

    }
    private void RefreshScores(){
        _currentScores[0] = _matchTeams.get(MatchTeam.Chaos).GetScore().GetCTFScore();
        _currentScores[1] = _matchTeams.get(MatchTeam.Balance).GetScore().GetCTFScore();
        _currentScores[2] = _matchTeams.get(MatchTeam.Order).GetScore().GetCTFScore();
    }
    public void FlagTaken(MatchCharacter verified, byte flagTaken, byte takerID){
        if(verified.IsAlive() && !IsHoldingFlag(takerID)){
            Flag taken = _flags.get(flagTaken);
            if(!taken.IsHeld()){
                taken.FlagTaken(takerID);
                SendToAll(Packets.FlagTakenPacket(flagTaken, takerID));
            }
        }

    }
    private void FlagDropped(Flag droppedFlag, MatchCharacter killedPlayer, byte killerID){
        Main.LogMessage("FlagDropped: " + killedPlayer.GetIDinMatch() + ", " + killerID);
        if(droppedFlag.IsHeld()){
            droppedFlag.FlagDropped(killedPlayer.GetPosition());
            _flagsChanged = true;
            SendToAll(Packets.FlagDroppedPacket(killedPlayer.GetIDinMatch(), droppedFlag.GetFlagBytes(), killerID));
        }
    }
    public void FlagReturned(byte flag){
        Flag returned = _flags.get(flag);
        if(!returned.IsHeld()){
            returned.FlagReturned();
            _flagsChanged = true;
            SendToAll(Packets.FlagReturnedPacket(flag));
        }
    }
    public void FlagReturnedByPlayer(MatchCharacter verified, byte flag){
        if(verified.IsAlive()){
            FlagReturned(flag);
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

    public boolean SeeIfFlagDropped(MatchCharacter character, byte killerID){
        for(Flag flag : _flags.values()){
            if(flag.GetHolderID() == character.GetIDinMatch()){
                FlagDropped(flag, character, killerID);
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
                    FlagReturned(flag.GetTeamID());
                }
            }
        }
    }
    @Override
    public boolean ParseCommand(String command, String[] params, MatchCharacter verified, byte senderID){
        if(!super.ParseCommand(command, params, verified, senderID)){
            switch(command){
                case "dropflag":
                    SeeIfFlagDropped(verified, senderID);
                    return true;
            }
        }
        return false;
    }

    @Override
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID) {
        MatchCharacter mc = super.JoinMatch(rc, teamID);
        GameServer.PGP.EnqueueForSendWithChecksum(Packets.CTFEntryPacket(_sceneID, mc, teamID, _matchPort, _processor.GetSymmetricKey(),
                (byte)_objectID, _matchType, _expirationTime), rc);
        return mc;
    }

    @Override
    protected void PlayerKilled(MatchCharacter killed, MatchCharacter killer){
        SeeIfFlagDropped(killed, killer.GetIDinMatch());
        super.PlayerKilled(killed, killer);
    }

    @Override
    public void LeaveMatch(MatchCharacter verified, boolean quitGame){
        Main.LogMessage("Player " + verified.GetIDinMatch() + " is leaving CTF match.");
        SeeIfFlagDropped(verified, (byte)0);
        super.LeaveMatch(verified, quitGame);
    }

    @Override
    public void Tick(long msElapsed)
    {
        DropCountdown(msElapsed);
        super.Tick(msElapsed);
    }

}
