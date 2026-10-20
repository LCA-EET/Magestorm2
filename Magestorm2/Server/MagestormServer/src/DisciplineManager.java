import java.sql.Connection;
import java.util.concurrent.ConcurrentHashMap;

public class DisciplineManager {

    private static ConcurrentHashMap<Byte, DisciplineData> _disciplines;
    private static byte[] _disciplinePacket;
    public static void init(Connection conn){
        _disciplines = new ConcurrentHashMap<>();
        Database.LoadDisciplineData(conn);
    }
    public static void AddDiscipline(byte code,DisciplineData toAdd){
        _disciplines.put(code, toAdd);
    }
    public static byte[] GetDisciplinePacket(){
        return _disciplinePacket;
    }
    public static void GenerateClientDataPacket(){
        _disciplinePacket = new byte[2 + _disciplines.size() * 4];
        _disciplinePacket[0] = Pregame_Send.DisciplineData;
        _disciplinePacket[1] = (byte)_disciplines.size();
        int index = 2;
        for(DisciplineData dd : _disciplines.values()){
            byte[] data = dd.ClientData();
            System.arraycopy(data, 0, _disciplinePacket, index, 4);
            index += 4;
        }
        _disciplinePacket = Cryptographer.ApplyChecksum(_disciplinePacket);
    }
    public static DisciplineData GetDiscipline(byte code){
        return _disciplines.get(code);
    }
}
