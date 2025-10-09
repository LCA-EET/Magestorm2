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
    private short _currentHealth;

    public MatchCharacter(PlayerCharacter pc, byte teamID, byte idInMatch, Match match){
        MarkPacketReceived();
        _currentHealth = 1;
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

    public boolean IsAlive(){
        return _currentHealth > 0;
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

    @Override
    public String toString(){
        return "MCID: " + _idInMatch + ", TeamID: " + _teamID;
    }
}
