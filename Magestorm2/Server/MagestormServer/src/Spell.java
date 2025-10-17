import java.util.concurrent.ConcurrentHashMap;

public class Spell {
    private final int _spellID;
    private final String _spellName;
    private final ConcurrentHashMap<Byte, Byte> _attributes;

    private byte _spellCost, _spellType;

    public Spell(int id, String name, byte[] params){
        _attributes = new ConcurrentHashMap<>();
        _spellID = id;
        _spellName = name;
        _spellCost = _spellType = -1;
        for(int i = 0; i < params.length; i += 2){
            _attributes.put(params[i], params[i+1]);
        }
    }
    public int GetSpellID(){
        return _spellID;
    }
    public String GetSpellName(){
        return _spellName;
    }
    public byte GetAttribute(byte key){
        return _attributes.get(key);
    }
    public byte SpellCost(){
        if(_spellCost == -1){
            _spellCost = _attributes.get(SpellAttributes.SpellCost);
        }
        return _spellCost;
    }
    public byte SpellType(){
        if(_spellType == -1){
            _spellType = _attributes.get(SpellAttributes.SpellType);
        }
        return _spellType;
    }
}
