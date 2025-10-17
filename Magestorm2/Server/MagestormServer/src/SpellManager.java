import java.util.concurrent.ConcurrentHashMap;

public class SpellManager {
    private static ConcurrentHashMap<Integer, Spell> _spellTable;

    public static void init(){
        _spellTable = new ConcurrentHashMap<>();
        Database.LoadSpellData();
    }

    public static void AddSpell(Spell toAdd){
        _spellTable.put(toAdd.GetSpellID(), toAdd);
    }

    public static Spell GetSpell(int id){
        return _spellTable.get(id);
    }
}
