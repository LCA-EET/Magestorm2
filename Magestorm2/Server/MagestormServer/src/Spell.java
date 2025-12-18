public class Spell {
    private final int _spellID;

    private final byte _minDamagePerRoll, _maxDamagePerRoll, _minHealPerRoll, _maxHealPerRoll, _element, _spellCost,
            _spellType, _discipline, _skillRequired, _numRolls;

    public Spell(int id, byte[] params){
        _spellID = id;
        _minDamagePerRoll = params[0];
        _maxDamagePerRoll = params[1];
        _minHealPerRoll = params[2];
        _maxHealPerRoll = params[3];
        _element = params[4];
        _spellCost = params[5];
        _spellType = params[6];
        _discipline = params[7];
        _skillRequired = params[8];
        _numRolls = params[9];
    }
    public int GetSpellID(){
        return _spellID;
    }
    public byte GetMinDamagePerRoll(){
        return _minDamagePerRoll;
    }
    public byte GetMaxDamagePerRoll(){
        return _maxDamagePerRoll;
    }
    public byte GetMinHealPerRoll(){
        return _minHealPerRoll;
    }
    public byte GetMaxHealPerRoll(){
        return _maxHealPerRoll;
    }
    public byte GetElement(){
        return _element;
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
}
