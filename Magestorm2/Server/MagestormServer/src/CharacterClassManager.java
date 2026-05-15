import java.sql.Connection;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;

public class CharacterClassManager {
    private static ConcurrentHashMap<Byte, CharacterClass> _characterClasses;
    private static ConcurrentHashMap<Byte, List<DisciplineData>> _disciplinesOfClass;
    public static void init(Connection conn){
        _characterClasses = new ConcurrentHashMap<>();
        _disciplinesOfClass = new ConcurrentHashMap<>();
        Database.LoadClassData(conn);
    }

    public static void AddDisciplineToClass(Byte classCode, DisciplineData disciplineData){
        if(!_disciplinesOfClass.containsKey(classCode)){
            _disciplinesOfClass.put(classCode, new ArrayList<>());
        }
        _disciplinesOfClass.get(classCode).add(disciplineData);
    }
    public static CharacterClass GetCharacterClass(byte classCode){
        return _characterClasses.get(classCode);
    }
    public static void AddCharacterClass(byte classCode, CharacterClass characterClass){
        _characterClasses.put(classCode, characterClass);
    }
    public static ArrayList<Byte> GetClassCodes(){
        return new ArrayList<>(_characterClasses.keySet());
    }
    public static byte[] GetDisciplinesOfClass(byte classCode){
        List<DisciplineData> data =  _disciplinesOfClass.get(classCode);
        byte[] toReturn = new byte[data.size()];
        byte index = 0;
        for(DisciplineData d : data){
            toReturn[index] = d.GetDisciplineID();
            index++;
        }
        return toReturn;
    }
}
