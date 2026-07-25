public class Spell {
    private final int _spellID;

    private final byte _minDamagePerRoll0, _maxDamagePerRoll0, _minHealPerRoll, _maxHealPerRoll, _element0, _spellCost,
            _spellType, _disciplineCode, _skillRequired, _numRolls, _minlevel, _minDamagePerRoll1, _maxDamagePerRoll1, _element1,
            _notificationCode, _radius, _effectCode, _duration, _levelsForRoll, _iceResist, _fireResist, _elecResist, _earthResist,
            _manaResist, _minForcePerRoll, _maxForcePerRoll, _forceDuration;
    private final DisciplineData _associatedDiscipline;
    private final float _splashFactor0, _splashFactor1, _splashFactor2;
    private final float[] _resistances;
    public Spell(int id, byte[] params){
        _spellID = id;
        _resistances = new float[10];
        _minDamagePerRoll0 = params[0];
        _maxDamagePerRoll0 = params[1];
        _minHealPerRoll = params[2];
        _maxHealPerRoll = params[3];
        _element0 = params[4];
        _spellCost = params[5];
        _spellType = params[6];
        _disciplineCode = params[7];
        _associatedDiscipline = DisciplineManager.GetDiscipline(_disciplineCode);
        _skillRequired = params[8];
        _numRolls = params[9];
        _minlevel = params[10];
        _minDamagePerRoll1 = params[11];
        _maxDamagePerRoll1 = params[12];
        _element1 = params[13];
        _levelsForRoll = params[14];
        _notificationCode = params[15];
        _splashFactor0 = params[16] / 100.0f;
        _splashFactor1 = params[17] / 100.0f;
        _splashFactor2 = params[18] / 100.0f;
        _radius = params[19];
        _effectCode = params[23];
        _duration = params[24];
        _iceResist = params[25];
        _fireResist = params[26];
        _elecResist = params[27];
        _earthResist = params[28];
        _manaResist = params[29];
        _minForcePerRoll = params[30];
        _maxForcePerRoll = params[31];
        _forceDuration = params[32];
        _resistances[ControlCodes.Element_Fire] = _fireResist / 100.0f;
        _resistances[ControlCodes.Element_Earth] = _earthResist / 100.0f;
        _resistances[ControlCodes.Element_Ice] = _iceResist / 100.0f;
        _resistances[ControlCodes.Element_Electric] = _elecResist / 100.0f;
        _resistances[ControlCodes.Element_Mana] = _manaResist / 100.0f;
    }

    public float[] GetResistances(){
        return _resistances;
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
    public byte GetMinForcePerRoll(){
        return _minForcePerRoll;
    }
    public byte GetMaxForcePerRoll(){
        return _maxForcePerRoll;
    }
    public byte GetForceDuration(){
        return _forceDuration;
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
    public DisciplineData GetDiscipline(){
        return _associatedDiscipline;
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
    public byte GetEffectCode(){
        return _effectCode;
    }
    public byte GetDuration(){
        return _duration;
    }
    public byte GetDisciplineCode(){
        return _disciplineCode;
    }
    public float GetSplashFactor(byte skillLevel){
        return switch (skillLevel) {
            case ControlCodes.SkillLevel_Basic -> _splashFactor0;
            case ControlCodes.SkillLevel_Expert -> _splashFactor1;
            case ControlCodes.SkillLevel_Master -> _splashFactor2;
            default -> 0;
        };
    }
}
