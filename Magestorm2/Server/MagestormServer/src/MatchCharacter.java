public class MatchCharacter {
    private PlayerCharacter _pc;

    private final byte _teamID;
    private final byte _matchID;
    private final byte _idInMatch;

    private final byte[] _NLCT;

    public MatchCharacter(int characterID, byte teamID, byte matchID, byte idInMatch){
        _pc = CharacterManager.GetCharacter(characterID);
        _teamID = teamID;
        _matchID = matchID;
        _idInMatch = idInMatch;
        byte[] nameLevelClass = _pc.GetNameLevelClassBytes();
        _NLCT = new byte[nameLevelClass.length + 1];
        _NLCT[0] = teamID;
        System.arraycopy(nameLevelClass, 0, _NLCT, 1, nameLevelClass.length);
    }

    public byte[] GetNLCTBytes(){
        return _NLCT;
    }

    public byte GetTeamID(){
        return _teamID;
    }

    public byte GetMatchID(){
        return _matchID;
    }

    public byte GetIDinMatch(){
        return _idInMatch;
    }
}
