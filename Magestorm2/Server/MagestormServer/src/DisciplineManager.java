import java.sql.Connection;
import java.util.concurrent.ConcurrentHashMap;

public class DisciplineManager {

    private static ConcurrentHashMap<Byte, DisciplineData> _disciplines;

    public static void init(Connection conn){
        _disciplines = new ConcurrentHashMap<>();
        Database.LoadDisciplineData(conn);
    }
    public static void AddDiscipline(byte code,DisciplineData toAdd){
        _disciplines.put(code, toAdd);
    }
    public static DisciplineData GetDiscipline(byte code){
        return _disciplines.get(code);
    }
}
