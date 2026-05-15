import java.sql.Connection;
import java.util.concurrent.ConcurrentHashMap;

public class EffectManager {
    private static ConcurrentHashMap<Byte, Effect> _effectTable;
    public static void init(Connection conn){
        _effectTable = new ConcurrentHashMap<>();
        Database.LoadEffectData(conn);
    }

    public static void AddEffect(Effect toAdd){
        Main.LogMessage("Added effect " + toAdd.GetEffectCode());
        _effectTable.put(toAdd.GetEffectCode(), toAdd);
    }

    public static Effect GetEffect(byte code){
        return _effectTable.get(code);
    }
    public static boolean ContainsEffect(byte code){
        return _effectTable.containsKey(code);
    }
}
