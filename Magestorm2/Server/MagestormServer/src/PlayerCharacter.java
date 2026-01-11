import java.util.Hashtable;

public class PlayerCharacter {
    private final String _characterName;
    private final int _characterID;
    private int _experience;
    private final CharacterClass _characterClass;
    private byte _level;
    private final byte _strength;
    private final byte _dexterity;
    private final byte _constitution;
    private final byte _charisma;
    private final byte _wisdom;
    private final byte _intellect;
    private final byte[] _characterBytes;
    private final byte[] _nameBytes;
    private final byte[] _nameLevelClass;
    private final byte[] _appearanceBytes;
    private final int _indexExperience = 17;
    private final int _indexSkills = 21;
    private final int _indexNameLength = 35;
    private final int _indexLevel = 16;
    private final int _indexSlotStart = 25;
    private final int _accountID;
    private final RemoteClient _remoteClient;
    private final Hashtable<Byte, Byte> _skillsTable;

    private byte _currentMatchID, _idInCurrentMatch, _currentTeam;
    private boolean _inMatch;
    private boolean[] _skills;
    public PlayerCharacter(byte[] fetched, int accountID){
        _inMatch = false;
        _skillsTable = new Hashtable<>();
        _remoteClient = GameServer.GetClient(accountID);
        _accountID = accountID;
        _characterBytes = fetched;
        _characterID = ByteUtils.ExtractInt(fetched, 0);
        _characterClass = new CharacterClass(fetched[4]);
        _strength = fetched[5];
        _dexterity = fetched[6];
        _constitution = fetched[7];
        _intellect = fetched[8];
        _charisma = fetched[9];
        _wisdom = fetched[10];
        _appearanceBytes = new byte[5];
        _appearanceBytes[0] = fetched[11];
        _appearanceBytes[1] = fetched[12];
        _appearanceBytes[2] = fetched[13];
        _appearanceBytes[3] = fetched[14];
        _appearanceBytes[4] = fetched[15];
        _level = fetched[_indexLevel];
        _experience = ByteUtils.ExtractInt(fetched, _indexExperience);
        int skills = ByteUtils.ExtractInt(fetched, _indexSkills);
        byte nameLength = fetched[_indexNameLength];
        _nameBytes = new byte[nameLength];
        System.arraycopy(fetched, _indexNameLength + 1, _nameBytes, 0, nameLength);
        _characterName = ByteUtils.BytesToUTF8(_nameBytes);
        _nameLevelClass = new byte[1 + 1 + 1 + nameLength];
        _nameLevelClass[0] = _level;
        _nameLevelClass[1] = _characterClass.GetClass();
        _nameLevelClass[2] = nameLength;
        System.arraycopy(_nameBytes, 0, _nameLevelClass, 3, nameLength);
        UpdateSkills(skills);
        CharacterManager.AddToCache(this);
    }
    public void UpdateSkills(int skills){
        _skills = ByteUtils.IntegerToBoolArray(skills);
        Main.LogMessage("Skills int: " + skills + ", " + ByteUtils.BitsToInt(_skills));
        _skillsTable.clear();
        byte[] classSkills = CharacterClass.GetBaseSkills(_characterClass.GetClass());
        for(byte classSkill : classSkills){
            int skillIndex = classSkill * 2;
            boolean lsb = _skills[skillIndex];
            boolean msb = _skills[skillIndex + 1];
            byte value;
            if(!msb && !lsb){   // 00
                value = 0;
            }
            else if (!msb){     // 01
                value = 1;
            }
            else if (!lsb){     // 10
                value = 2;
            }
            else{               // 11
                value = 3;
            }
            _skillsTable.put(classSkill, value);
        }
        Main.LogMessage("Skills Table");
        for(Byte key : _skillsTable.keySet()){
            Main.LogMessage(key + ":" + _skillsTable.get(key));
        }
    }
    public void UpdateSlottedSpells(byte[] slots){
        System.arraycopy(slots, 0, _characterBytes, _indexSlotStart, 10);
    }
    public byte GetMaxStamina(){
        return (byte)(85.0f + (_strength * 8.5f));
    }
    public float GetMaxHP(){
        short multiplier = _characterClass.HPMultiplier();
        float toReturn = (_level * (_constitution / 20.0f) * multiplier * 1.579f) + 10;
        return Math.round(toReturn);
    }
    public float GetMaxMana(){
        byte statToUse = _characterClass.IsCleric() ? _charisma : _intellect;
        float manaMultiplier = 1 + ((statToUse - 10) * 0.05f);
        return ((_level * 4) + 10) * manaMultiplier;
    }
    public String GetCharacterName(){
        return _characterName;
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
    public byte[] GetAppearanceBytes(){
        return _appearanceBytes;
    }
    public int GetAccountID(){
        return _accountID;
    }
    public RemoteClient GetRemoteClient(){
        return _remoteClient;
    }
    public void SetMatchDetails(byte id, byte match, byte team){
        _idInCurrentMatch = id;
        _currentMatchID = match;
        _currentTeam = team;
        _inMatch = true;
        GameServer.AddActiveCharacter(_accountID, this);
    }
    public byte GetIDinMatch(){
        return _idInCurrentMatch;
    }
    public byte GetMatchID(){
        return _currentMatchID;
    }
    public byte GetCurrentTeam(){
        return _currentTeam;
    }
    public boolean IsInMatch(){
        return _inMatch;
    }
    public void MarkRemovedFromMatch(){
        _inMatch = false;
        _idInCurrentMatch = 0;
        _currentMatchID = 0;
        _currentTeam = 0;
        GameServer.RemoveActiveCharacter(_accountID);
    }
    public CharacterClass GetCharacterClass(){
        return _characterClass;
    }
    public byte GetSkillLevel(byte discipline)
    {
        byte skillLevel = 0;
        if(_skillsTable.containsKey(discipline)){
            skillLevel = _skillsTable.get(discipline);
        }
        return skillLevel;
    }

}
