public class PlayerCharacter {
    private String _characterName;
    private int _characterID;
    private int _experience;
    private byte _characterClass;
    private byte _level;
    private byte _strength;
    private byte _dexterity;
    private byte _constitution;
    private byte _charisma;
    private byte _wisdom;
    private byte _intellect;
    private byte _appsex;
    private byte _appskin;
    private byte _apphair;
    private byte _appface;
    private byte _apphead;
    private byte[] _characterBytes;
    private byte[] _nameBytes;
    private byte[] _nameLevelClass;
    private final int _indexExperience = 17;
    private final int _indexLevel = 16;

    public PlayerCharacter(byte[] fetched){
        _characterBytes = fetched;
        _characterID = ByteUtils.ExtractInt(fetched, 0);
        _characterClass = fetched[4];
        _strength = fetched[5];
        _dexterity = fetched[6];
        _constitution = fetched[7];
        _intellect = fetched[8];
        _charisma = fetched[9];
        _wisdom = fetched[10];
        _appsex = fetched[11];
        _appskin = fetched[12];
        _apphair = fetched[13];
        _appface = fetched[14];
        _apphead = fetched[15];
        _level = fetched[_indexLevel];
        _experience = ByteUtils.ExtractInt(fetched, _indexExperience);
        byte nameLength = fetched[21];
        _nameBytes = new byte[nameLength];
        System.arraycopy(fetched, 22, _nameBytes, 0, nameLength);
        _characterName = ByteUtils.BytesToUTF8(_nameBytes);
        _nameLevelClass = new byte[1 + 1 + 1 + nameLength];
        _nameLevelClass[0] = _level;
        _nameLevelClass[1] = _characterClass;
        _nameLevelClass[2] = nameLength;
        System.arraycopy(_nameBytes, 0, _nameLevelClass, 3, nameLength);
        CharacterManager.AddToCache(this);
    }
    public void UpdateExperience(int experience){
        _experience = experience;
        byte[] expBytes = ByteUtils.IntToByteArray(experience);
        System.arraycopy(expBytes,0,_characterBytes, _indexExperience, 8);
    }
    public void UpdateLevel(byte level){
        _level = level;
        _characterBytes[_indexLevel] = _level;
    }

    public int GetCharacterID(){
        return _characterID;
    }

    public byte[] GetNameLevelClassBytes(){
        return _nameLevelClass;
    }
    public byte[] GetCharacterBytes(){
        return _characterBytes;
    }
    public byte[] GetNameBytes(){
        return _nameBytes;
    }

}
