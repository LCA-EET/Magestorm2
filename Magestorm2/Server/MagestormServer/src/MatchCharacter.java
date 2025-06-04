public class MatchCharacter {
    private PlayerCharacter _pc;

    private final byte _teamID;
    private final byte _idInMatch;

    private final byte[] _INLCT;

    public MatchCharacter(PlayerCharacter pc, byte teamID, byte idInMatch){
        _pc = pc;
        _teamID = teamID;
        _idInMatch = idInMatch;
        byte[] nameLevelClass = _pc.GetNameLevelClassBytes();
        _INLCT = new byte[nameLevelClass.length + 2];
        _INLCT[0] = idInMatch;
        _INLCT[1] = teamID;
        System.arraycopy(nameLevelClass, 0, _INLCT, 2, nameLevelClass.length);
    }

    public byte[] GetINLCTBytes(){
        return _INLCT;
    }

    public byte GetTeamID(){
        return _teamID;
    }

    public byte GetIDinMatch(){
        return _idInMatch;
    }
}
