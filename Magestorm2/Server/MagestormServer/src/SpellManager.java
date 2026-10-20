import java.sql.Connection;
import java.util.Collection;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.concurrent.ConcurrentHashMap;

public class SpellManager {
    private static ConcurrentHashMap<Integer, Spell> _spellTable;
    private static ConcurrentHashMap<Byte, byte[]> _spellData;
    public static void init(Connection conn){
        _spellTable = new ConcurrentHashMap<>();
        _spellData = new ConcurrentHashMap<>();
        Database.LoadSpellData(conn);
    }

    public static void AddSpell(Spell toAdd){
        //Main.LogMessage("Added spell " + toAdd.GetSpellID());
        _spellTable.put(toAdd.GetSpellID(), toAdd);
    }
    private static void SaveSpellPacket(byte index, int length, ArrayList<byte[]> data){
        byte[] toSave = new byte[3 + length];
        toSave[0] = Pregame_Send.SpellData;
        toSave[1] = (byte)_spellTable.size();
        toSave[2] = (byte)data.size();
        int arrayIdx = 3;
        for(byte[] spellData : data){
            System.arraycopy(spellData, 0, toSave, arrayIdx, spellData.length);
            arrayIdx += spellData.length;
        }
        _spellData.put(index, Cryptographer.ApplyChecksum(toSave));
    }
    public static Collection<byte[]> GetSpellDataPackets(){
        return _spellData.values();
    }
    public static void GenerateClientDataPackets(){
        ArrayList<byte[]> bytes = new ArrayList<>();
        byte index = 0;
        int length = 0;
        for(Spell spell : _spellTable.values()){
            byte[] toAdd = spell.GetClientSpellData();
            if(length + toAdd.length >= GameServer.MaxUDPPayload - 5){
                SaveSpellPacket(index, length, bytes);
                length = 0;
                bytes.clear();
                index++;
            }
            length += toAdd.length;
            bytes.add(toAdd);
        }
        if(!bytes.isEmpty()){
            SaveSpellPacket(index, length, bytes);
        }
    }

    public static Spell GetSpell(int id){
        return _spellTable.get(id);
    }
    public static boolean ContainsSpell(int spellKey){
        return _spellTable.containsKey(spellKey);
    }
}
