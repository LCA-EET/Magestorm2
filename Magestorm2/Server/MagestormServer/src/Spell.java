public class Spell {
    private final int _spellID;

    private final byte _minDamagePerRoll0, _maxDamagePerRoll0, _minHealPerRoll, _maxHealPerRoll, _element0, _spellCost,
            _spellType, _discipline, _skillRequired, _numRolls, _minlevel, _minDamagePerRoll1, _maxDamagePerRoll1, _element1,
            _notificationCode, _effectRadius;
    private final float _splashFactor0, _splashFactor1, _splashFactor2;

    public Spell(int id, byte[] params){
        _spellID = id;
        _minDamagePerRoll0 = params[0];
        _maxDamagePerRoll0 = params[1];
        _minHealPerRoll = params[2];
        _maxHealPerRoll = params[3];
        _element0 = params[4];
        _spellCost = params[5];
        _spellType = params[6];
        _discipline = params[7];
        _skillRequired = params[8];
        _numRolls = params[9];
        _minlevel = params[10];
        _minDamagePerRoll1 = params[11];
        _maxDamagePerRoll1 = params[12];
        _element1 = params[13];
        _notificationCode = params[14];
        _splashFactor0 = params[15] / 100.0f;
        _splashFactor1 = params[16] / 100.0f;
        _splashFactor2 = params[17] / 100.0f;
        _effectRadius = params[18];
    }
    public boolean IsDamaging(){
        return _minDamagePerRoll0 > 0;
    }
    public boolean IsHealing(){
        return _minHealPerRoll > 0;
    }
    public int GetSpellID(){
        return _spellID;
    }
    public byte GetMinDamagePerRoll0(){
        return _minDamagePerRoll0;
    }
    public byte GetMaxDamagePerRoll0(){
        return _maxDamagePerRoll0;
    }
    public byte GetMinHealPerRoll(){
        return _minHealPerRoll;
    }
    public byte GetMaxHealPerRoll(){
        return _maxHealPerRoll;
    }
    public byte GetElement0(){
        return _element0;
    }
    public byte SpellCost(){
        return _spellCost;
    }
    public byte SpellType(){
        return _spellType;
    }
    public byte GetDiscipline(){
        return _discipline;
    }
    public byte GetSkillRequired(){
        return _skillRequired;
    }
    public byte GetNumRolls(){
        return _numRolls;
    }
    public byte GetMinLevel() {return _minlevel;}
    public byte GetMinDamagePerRoll1(){
        return _minDamagePerRoll1;
    }
    public byte GetMaxDamagePerRoll1(){
        return _maxDamagePerRoll1;
    }
    public byte GetElement1(){
        return _element1;
    }
    public byte GetNotificationCode(){return _notificationCode;}

    public float GetSplashFactor(byte skillLevel){
        return switch (skillLevel) {
            case ControlCodes.SkillLevel_Basic -> _splashFactor0;
            case ControlCodes.SkillLevel_Expert -> _splashFactor1;
            case ControlCodes.SkillLevel_Master -> _splashFactor2;
            default -> 0;
        };
    }
}
