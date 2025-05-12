public class MatchPlayer {
    private RemoteClient _owner;
    private int _characterID;
    private byte _idInMatch;
    private String _characterName;
    private byte[] _nameBytes;
    private byte _teamID;
    private byte[] _playerBytes;

    public MatchPlayer(RemoteClient owner, int characterID, byte idInMatch, String characterName, byte[] nameBytes, byte teamID){
        _owner = owner;
        _characterID = characterID;
        _idInMatch = idInMatch;
        _characterName = characterName;
        _nameBytes = nameBytes;
        _teamID = teamID;
        _playerBytes = new byte[1 + 1 + 1 + nameBytes.length];
        _playerBytes[0] = idInMatch;
        _playerBytes[1] = teamID;
        _playerBytes[2] = (byte)_nameBytes.length;
        System.arraycopy(nameBytes, 0, _playerBytes, 3, nameBytes.length);
    }

    public byte IDinMatch(){
        return _idInMatch;
    }

    public byte[] CharacterNameBytes(){
        return _nameBytes;
    }

    public byte TeamID()
    {
        return _teamID;
    }

    public byte[] PlayerBytes(){
        return _playerBytes;
    }

}
