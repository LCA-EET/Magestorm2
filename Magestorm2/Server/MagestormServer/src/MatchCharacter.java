public class MatchCharacter {
    private final Match _owningMatch;

    private final PlayerCharacter _pc;

    private final byte _teamID;
    private final byte _idInMatch;

    private final byte[] _INLCTA;
    private boolean _verified;

    private long _lastPacketReceived;
    private final long _inactivityWarningThreshold = 30000;
    private final long _inactivityMaximumThreshold = 61000;
    private short _currentHP, _currentMana;

    public MatchCharacter(PlayerCharacter pc, byte teamID, byte idInMatch, Match match){
        MarkPacketReceived();
        _currentHP = 1;
        _currentMana = 1;
        _verified = false;
        _owningMatch = match;
        _pc = pc;
        _pc.SetMatchDetails(idInMatch, match.MatchID(), teamID);
        _teamID = teamID;
        _idInMatch = idInMatch;
        byte[] nameLevelClass = _pc.GetNameLevelClassBytes();
        _INLCTA = new byte[nameLevelClass.length + 7];
        _INLCTA[0] = idInMatch;
        _INLCTA[1] = teamID;
        byte[] appearanceBytes = pc.GetAppearanceBytes();
        System.arraycopy(appearanceBytes, 0, _INLCTA, 2, appearanceBytes.length);
        System.arraycopy(nameLevelClass, 0, _INLCTA, 7, nameLevelClass.length);
    }

    public void TakeDamage(short damageAmount, byte damageSource){
        _currentHP -= damageAmount;
        if(_currentHP <= 0){
            _owningMatch.SendToAll(Packets.PlayerKilledPacket(_idInMatch, damageSource));
            _owningMatch.AdjustScore(_idInMatch, -1);
            _owningMatch.AdjustScore(damageSource, 1);
        }
        else{
            _owningMatch.SendToPlayer(Packets.PlayerDamagedPacket(_idInMatch, damageSource, _currentHP), this);
        }
    }

    public short GetRemainingMana(){
        return _currentMana;
    }
    public boolean IsAlive(){
        return _currentHP > 0;
    }

    public PlayerCharacter PC(){
        return _pc;
    }

    public CharacterClass GetClass(){
        return _pc.GetCharacterClass();
    }

    public byte GetClassCode(){
        return _pc.GetCharacterClass().GetClass();
    }

    public byte[] GetINLCTABytes(){
        return _INLCTA;
    }

    public byte GetTeamID(){
        return _teamID;
    }

    public byte GetIDinMatch(){
        return _idInMatch;
    }

    public String GetCharacterName(){
        return _pc.GetCharacterName();
    }

    public void MarkVerified(){
        Main.LogMessage("Player " + _idInMatch + " verified for team " + _teamID);
        MarkPacketReceived();
        _verified = true;
    }

    public boolean IsVerified(){
        return _verified;
    }

    public RemoteClient GetRemoteClient(){
        return _pc.GetRemoteClient();
    }

    public void MarkPacketReceived(){
        _lastPacketReceived = System.currentTimeMillis();
    }

    public boolean InactivityExceededWarningThreshold(){
        return (System.currentTimeMillis() - _lastPacketReceived) >= _inactivityWarningThreshold;
    }

    public boolean InactivityExceededMaximumThreshold(){
        Main.LogMessage("Inactivity check: " + _lastPacketReceived + ", " + _inactivityMaximumThreshold);
        return (System.currentTimeMillis() - _lastPacketReceived) >= _inactivityMaximumThreshold;
    }

    public void AdjustMana(int adjustment, boolean notify){
        _currentMana += adjustment;
        if(notify){
            _owningMatch.SendToPlayer(Packets.HMLPacket(_currentHP, _currentMana, (byte)0), this);
        }
    }

    @Override
    public String toString(){
        return "MCID: " + _idInMatch + ", TeamID: " + _teamID;
    }
}
