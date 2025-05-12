import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.concurrent.ConcurrentHashMap;

public class Match {
    private byte _matchID;
    private int _creatorID;
    private byte _sceneID;
    private long _creationTime;
    private long _expirationTime;
    private String _creator;
    private byte[] _matchBytes;
    private byte _lastIndex;
    private ConcurrentHashMap<Byte, MatchTeam> _matchTeams;
    private byte _nextPlayerID;

    public Match(byte matchID, int creatorID, String creator, byte sceneID, long creationTime){
        InitTeams();
        _nextPlayerID = 0;
        _matchID = matchID;
        _creatorID = creatorID;
        _sceneID = sceneID;
        _creator = creator;
        _creationTime = creationTime;
        _expirationTime = creationTime + 3600000; // one hour
        Main.LogMessage("Initializing match " + _matchID + " with expiration time: " + _expirationTime);
        byte[] creatorNameBytes = creator.getBytes(StandardCharsets.UTF_8);
        byte nameBytesLength = (byte)creatorNameBytes.length;
        _matchBytes = new byte[1 + 1 + 8 + 4 + 1 +  nameBytesLength + 1];
        _lastIndex = (byte)(_matchBytes.length-1);
        int index = 0;
        _matchBytes[index] = matchID;
        index++;
        _matchBytes[index] = sceneID;
        index++;
        byte[] expirationBytes = ByteUtils.LongToByteArray(_expirationTime);
        System.arraycopy(expirationBytes, 0, _matchBytes, index, 8);
        index+=8;
        byte[] accountIDBytes = ByteUtils.IntToByteArray(creatorID);
        System.arraycopy(accountIDBytes, 0, _matchBytes, index, 4);
        index+=4;
        _matchBytes[index] = nameBytesLength;
        index++;
        System.arraycopy(creatorNameBytes, 0, _matchBytes, index, nameBytesLength);
    }
    private void InitTeams(){
        _matchTeams = new ConcurrentHashMap<>();
        for(byte teamID : MatchTeam.TeamCodes){
            _matchTeams.put(teamID, new MatchTeam(teamID));
        }
    }
    public byte MatchID(){
        return _matchID;
    }
    public int CreatorAccountID(){
        return _creatorID;
    }
    public byte NumPlayersInMatch(){
        byte totalPlayers = 0;
        for(MatchTeam team : _matchTeams.values()){
            totalPlayers += team.NumPlayers();
        }
        return totalPlayers;
    }
    public byte[] ToByteArray(){
        _matchBytes[_lastIndex] = NumPlayersInMatch();
        return _matchBytes;
    }
    public void JoinMatch(RemoteClient rc, byte teamID){
        rc.AssignToMatch(_matchID, teamID, ObtainNextPlayerID());
    }
    public byte[] PlayersInMatch(){
        ArrayList<byte[]> teamBytes = new ArrayList<>();
        int length = 0;
        for(byte teamID : MatchTeam.TeamCodes){
            byte[] teamPlayers = _matchTeams.get(teamID).GetPlayerBytes();
            teamBytes.add(teamPlayers);
            length+=teamPlayers.length;
        }
        return ByteUtils.ArrayListToByteArray(teamBytes, length, 0);
    }
    public byte ObtainNextPlayerID(){
        boolean idUsed = false;
        for(MatchTeam team : _matchTeams.values()){
            if(team.PlayerIDUsed(_nextPlayerID)){
               idUsed = true;
               break;
            }
        }
        if(idUsed){
            if(_nextPlayerID >= 100){
                _nextPlayerID = 0;
            }
            else{
                _nextPlayerID++;
            }
            return ObtainNextPlayerID();
        }
        else{
            byte toReturn = _nextPlayerID;
            _nextPlayerID ++;
            return toReturn;
        }
    }
}
