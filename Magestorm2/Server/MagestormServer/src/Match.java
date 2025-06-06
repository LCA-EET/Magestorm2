import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class Match {
    private byte _matchID;
    private int _creatorID;
    private byte _sceneID;
    private long _creationTime;
    private long _expirationTime;
    private byte[] _creatorName;
    private byte[] _matchBytes;
    private byte _lastIndex;
    private ConcurrentHashMap<Byte, MatchTeam> _matchTeams;
    private byte _nextPlayerID;
    private final int _matchPort;
    private final InGamePacketProcessor _processor;
    private final byte _maxPlayers;
    private byte _numPlayers;

    public Match(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime){
        InitTeams();
        _maxPlayers = GameServer.RetrieveMaxPlayerData(sceneID);
        _numPlayers = 0;
        _creatorName = creatorName;
        _nextPlayerID = 0;
        _matchID = matchID;
        _creatorID = creatorID;
        _sceneID = sceneID;
        _creationTime = creationTime;
        _expirationTime = creationTime + (3600000 / 60); // one hour
        _matchPort = GameServer.GetNextMatchPort();
        Main.LogMessage("Initializing match " + _matchID + " with expiration time: " + _expirationTime);
        byte nameBytesLength = (byte)_creatorName.length;
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
        System.arraycopy(_creatorName, 0, _matchBytes, index, nameBytesLength);
        _processor = new InGamePacketProcessor(_matchPort);
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
        return _numPlayers;
    }
    public byte[] ToByteArray(){
        _matchBytes[_lastIndex] = NumPlayersInMatch();
        return _matchBytes;
    }
    public boolean HasRoomForAnotherPlayer(){
        return _numPlayers < _maxPlayers;
    }
    public void LeaveMatch(RemoteClient rc){
        _numPlayers--;
        // TODO
    }
    public void JoinMatch(RemoteClient rc, byte teamID){
        byte playerID = ObtainNextPlayerID();
        MatchTeam matchTeam = _matchTeams.get(teamID);
        MatchCharacter toAdd = new MatchCharacter(rc.GetActiveCharacter(), teamID, playerID);
        matchTeam.AddPlayer(playerID, toAdd);
        _numPlayers++;
        GameServer.EnqueueForSend(Packets.MatchEntryPacket(_sceneID, teamID, playerID, _matchPort), rc);
    }
    public byte[] PlayersInMatch(byte opCode){
        ArrayList<byte[]> teamBytes = new ArrayList<>();
        int length = 2;
        for(byte teamID : MatchTeam.TeamCodes){
            byte[] teamPlayers = _matchTeams.get(teamID).GetPlayerBytes();
            teamBytes.add(teamPlayers);
            length+=teamPlayers.length;
        }
        byte[] toReturn = ByteUtils.ArrayListToByteArray(teamBytes, length, 2);
        toReturn[0] = opCode;
        toReturn[1] = _matchID;
        return toReturn;
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
    public long GetExpiration(){
        return _expirationTime;
    }
    public void MarkExpired(){
        MatchManager.RemoveMatch(_matchID);
        _processor.TerminateProcessor();
    }
}
