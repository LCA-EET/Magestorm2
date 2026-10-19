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
        byte[] toSave = new byte[4 + length];
        toSave[0] = Pregame_Send.SpellData;
        toSave[1] = index;
        toSave[3] = (byte)data.size();
        int arrayIdx = 4;
        for(byte[] spellData : data){
            System.arraycopy(spellData, 0, toSave, arrayIdx, spellData.length);
            arrayIdx += spellData.length;
        }
        _spellData.put(index, toSave);
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
            if(length + toAdd.length >= GameServer.MaxUDPPayload - 6){
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
        for(Byte packetID : _spellData.keySet()){
            byte[] spellDataPacket = _spellData.get(packetID);
            spellDataPacket[2] = index;
            _spellData.put(packetID, Cryptographer.ApplyChecksum(spellDataPacket));
        }
    }

    public static Spell GetSpell(int id){
        return _spellTable.get(id);
    }
    public static boolean ContainsSpell(int spellKey){
        return _spellTable.containsKey(spellKey);
    }
}
