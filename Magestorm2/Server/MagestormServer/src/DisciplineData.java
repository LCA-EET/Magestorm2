import java.util.ArrayList;

public class DisciplineData {
    private final byte _id, _availabilityMask, _statCode;

    public DisciplineData(byte disciplineID, byte[] attrib){
        _id = disciplineID;
        _availabilityMask = attrib[0];
        ParseMask();
        _statCode = attrib[1];
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
