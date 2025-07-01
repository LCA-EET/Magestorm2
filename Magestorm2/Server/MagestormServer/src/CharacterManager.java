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
}
