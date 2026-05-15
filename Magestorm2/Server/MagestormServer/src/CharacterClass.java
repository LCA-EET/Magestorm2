public class CharacterClass {
    public static final byte Arcanist = 0;
    public static final byte Cleric = 1;
    public static final byte Magician = 2;
    public static final byte Mentalist = 3;

    private final byte _classID, _hpMultiplier, _manaStatCode, _biasMultiplier, _shrineMultiplier, _poolBiasChance;

    public CharacterClass(byte id, byte[] attrib){
        _classID = id;
        _hpMultiplier = attrib[0];
        _manaStatCode = attrib[1];
        _biasMultiplier = attrib[3];
        _shrineMultiplier = attrib[4];
        _poolBiasChance = attrib[5];
    }
    public byte GetPoolBiasChance(){
        return _poolBiasChance;
    }

    public byte GetHPMultiplier(){
        return _hpMultiplier;
    }

    public byte GetManaStatCode(){
        return _manaStatCode;
    }

    public byte GetBiasMultiplier(){
        return _biasMultiplier;
    }

    public byte GetShrineMultiplier(){
        return _shrineMultiplier;
    }

    public byte GetClassID(){
        return _classID;
    }


    public boolean IsCleric(){
        return _classID == Cleric;
    }

    public boolean IsMagician(){
        return _classID == Magician;
    }



}
