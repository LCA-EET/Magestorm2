public class MatchCharacter {
    private PlayerCharacter _pc;

    private final byte _teamID;
    private final byte _idInMatch;

    private final byte[] _INLCTA;
    private boolean _verified;

    public MatchCharacter(PlayerCharacter pc, byte teamID, byte idInMatch){
        _verified = false;
        _pc = pc;
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

    public byte[] GetINLCTABytes(){
        return _INLCTA;
    }

    public byte GetTeamID(){
        return _teamID;
    }

    public byte GetIDinMatch(){
        return _idInMatch;
    }

    public void MarkVerified(){
        _verified = true;
    }

    public boolean IsVerified(){
        return _verified;
    }
}
