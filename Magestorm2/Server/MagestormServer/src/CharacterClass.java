public class CharacterClass {
    public static final byte Arcanist = 0;
    public static final byte Cleric = 1;
    public static final byte Magician = 2;
    public static final byte Mentalist = 3;

    private final byte _classID, _deadSight, _hpMultiplier, _manaStatCode, _biasMultiplier, _shrineMultiplier, _poolBiasChance;
    private byte[] _clientData;
    public CharacterClass(byte id, byte[] attrib, short nameRef){
        _classID = id;
        _hpMultiplier = attrib[0];
        _manaStatCode = attrib[1];
        _deadSight = attrib[2];
        _biasMultiplier = attrib[3];
        _shrineMultiplier = attrib[4];
        _poolBiasChance = attrib[5];
        SaveClientData(nameRef);
    }
    private void SaveClientData(short nameRef){
        _clientData = new byte[6];
        _clientData[0] = _classID;
        _clientData[1] = _hpMultiplier;
        _clientData[2] = _manaStatCode;
        _clientData[3] = _deadSight;
        byte[] nameRefBytes = ByteUtils.ShortToByteArray(nameRef);
        System.arraycopy(nameRefBytes, 0, _clientData, 4, 2);
    }
    public byte[] ClientData(){
        return _clientData;
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
