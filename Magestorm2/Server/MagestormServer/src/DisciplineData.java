import java.util.ArrayList;

public class DisciplineData {
    private final byte _id, _availabilityMask, _statCode;
    private byte[] _clientData;
    public DisciplineData(byte disciplineID, byte[] attrib, short nameRef){
        _id = disciplineID;
        _availabilityMask = attrib[0];
        ParseMask();
        _statCode = attrib[1];
        FillClientDataArray(nameRef);
    }
    private void FillClientDataArray(short nameRef){
        _clientData = new byte[4];
        _clientData[0] = _id;
        _clientData[1] = _availabilityMask;
        byte[] nameRefBytes = ByteUtils.ShortToByteArray(nameRef);
        System.arraycopy(nameRefBytes, 0, _clientData, 2, 2);
    }
    public byte[] ClientData(){
        return _clientData;
    }
    public byte GetDisciplineID(){
        return _id;
    }

    public byte GetAvailabilityMask(){
        return _availabilityMask;
    }

    public byte GetStatCode(){
        return _statCode;
    }
    private void ParseMask(){
        ArrayList<Byte> classCodes = CharacterClassManager.GetClassCodes();
        boolean[] bools = ByteUtils.BytesToBooleanArray(new byte[]{_availabilityMask});
        for(byte classCode : classCodes){
            if(bools[classCode]){
                CharacterClassManager.AddDisciplineToClass(classCode, this);
            }
        }
    }
}
