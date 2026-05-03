import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class CharacterManager {
    public static ConcurrentHashMap<Integer, PlayerCharacter> _cachedCharacters;

    public static void init(){
        _cachedCharacters = new ConcurrentHashMap<>();
    }
    public static void AddToCache(PlayerCharacter toAdd){
        _cachedCharacters.put(toAdd.GetCharacterID(), toAdd);
        Main.LogMessage("Adding cached character " + toAdd.GetCharacterID() + ", " + toAdd.GetCharacterName());
    }

    public static PlayerCharacter GetCharacter(int id){
        if(_cachedCharacters.containsKey(id)){
            return _cachedCharacters.get(id);
        }
        return null;
    }

    public static boolean CharacterBelongsToAccount(int characterID, int accountID){
        return _cachedCharacters.get(characterID).GetAccountID() == accountID;
    }

    public static byte[] GetCharactersOfAccount(int accountID){
        ArrayList<byte[]> pcs = new ArrayList<>();
        byte numCharacters = 0;
        int totalSize = 2;
        for(PlayerCharacter pc : _cachedCharacters.values()){
            if(pc.GetAccountID() == accountID){
                byte[] cb = pc.GetCharacterBytes();
                numCharacters++;
                totalSize += cb.length;
                pcs.add(cb);
            }
        }
        byte[] toReturn = new byte[totalSize];
        toReturn[1] = numCharacters;
        int index = 2;
        for(byte[] pcBytes : pcs){
            System.arraycopy(pcBytes, 0, toReturn, index, pcBytes.length);
            index += pcBytes.length;
        }
        return toReturn;
    }
}
