import java.sql.Connection;
import java.util.concurrent.ConcurrentHashMap;

public class SpellManager {
    private static ConcurrentHashMap<Integer, Spell> _spellTable;
    public static void init(Connection conn){
        _spellTable = new ConcurrentHashMap<>();
        Database.LoadSpellData(conn);
    }

    public static void AddSpell(Spell toAdd){
        Main.LogMessage("Added spell " + toAdd.GetSpellID());
        _spellTable.put(toAdd.GetSpellID(), toAdd);
    }

    public static Spell GetSpell(int id){
        return _spellTable.get(id);
    }
    public static boolean ContainsSpell(int spellKey){
        return _spellTable.containsKey(spellKey);
    }
}
