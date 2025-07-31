import java.awt.color.ICC_ProfileGray;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.concurrent.ConcurrentHashMap;

public class Match {
    private final byte _matchID;
    private final int _creatorID;
    private final byte _sceneID;
    private final long _expirationTime;
    private final byte[] _creatorName;
    private final byte[] _matchBytes;
    private final byte _lastIndex;
    private ConcurrentHashMap<Byte, MatchTeam> _matchTeams;
    private final ConcurrentHashMap<Byte, MatchCharacter> _matchCharacters;
    private final ConcurrentHashMap<Byte, RemoteClient> _verifiedClients;
    private final ConcurrentHashMap<Byte, ActivatableObject> _objectStatus;
    private byte _nextPlayerID;
    private final int _matchPort;
    private final InGamePacketProcessor _processor;
    private final byte _maxPlayers;



    public Match(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration){
        _objectStatus = new ConcurrentHashMap<>();
        _matchCharacters = new ConcurrentHashMap<>();
        _maxPlayers = GameServer.RetrieveMaxPlayerData(sceneID);
        _creatorName = creatorName;
        _nextPlayerID = 0;
        _matchID = matchID;
        _creatorID = creatorID;
        _sceneID = sceneID;
        _expirationTime = creationTime + (3600000 - (duration * 900000)); // 0 = one hour
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
        _verifiedClients = new ConcurrentHashMap<>();
        InitTeams();
        _processor = new InGamePacketProcessor(_matchPort, this);
    }
    public MatchTeam GetMatchTeam(byte teamID){
        return _matchTeams.get(teamID);
    }
    public void ChangeObjectState(byte objectID, byte status){
        if(_objectStatus.containsKey(objectID)){
            _objectStatus.get(objectID).ChangeState(status);
        }
        else{
            _objectStatus.put(objectID, new ActivatableObject(objectID, status));
        }
    }


    public byte[] GetObjectStatusBytes(){

        int length = _objectStatus.size() * 2;
        byte[] toReturn = new byte[length];
        int index = 0;
        for(byte k : _objectStatus.keySet()){
            toReturn[index] = k;
            index++;
            //toReturn[index] = _objectStatus.get(k);
            index++;
        }
        return toReturn;
    }

    private void InitTeams(){
        _matchTeams = new ConcurrentHashMap<>();
        for(byte teamID : MatchTeam.TeamCodes){
            _matchTeams.put(teamID, new MatchTeam(teamID, this));
        }
        Main.LogMessage("Teams initialized for match " + _matchID + ".");
    }
    public byte MatchID(){
        return _matchID;
    }
    public int CreatorAccountID(){
        return _creatorID;
    }
    public byte NumPlayersInMatch(){
        return (byte)_matchCharacters.size();
    }
    public byte[] ToByteArray(){
        _matchBytes[_lastIndex] = NumPlayersInMatch();
        return _matchBytes;
    }
    public boolean HasRoomForAnotherPlayer(){
        return NumPlayersInMatch() < _maxPlayers;
    }
    public void JoinMatch(RemoteClient rc, byte teamID){
        byte playerID = ObtainNextPlayerID();
        MatchTeam matchTeam = _matchTeams.get(teamID);
        MatchCharacter toAdd = new MatchCharacter(rc.GetActiveCharacter(), teamID, playerID);
        matchTeam.AddPlayer(playerID, toAdd);
        _matchCharacters.put(playerID, toAdd);

        Main.LogMessage("Match " + _matchID +": Added player " + playerID + " to team " + teamID);
        GameServer.EnqueueForSend(Packets.MatchEntryPacket(_sceneID, teamID, playerID, _matchPort), rc);
    }
    public void LeaveMatch(byte id, byte team){
        _matchCharacters.remove(id);
        _verifiedClients.remove(id);
        _matchTeams.get(team).RemovePlayer(id);
        SendToAll(Packets.PlayerLeftMatchPacket(id, team));
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
    private byte ShrineHealth(byte teamID){
        return _matchTeams.get(teamID).ShrineHealth();
    }
    public byte[] ReportAllShrineHealth(){
        return new byte[]{
                ShrineHealth(MatchTeam.Chaos) ,
                ShrineHealth(MatchTeam.Balance),
                ShrineHealth(MatchTeam.Order)
        };
    }
    public byte[] PlayerData(byte idInMatch){
        return _matchCharacters.get(idInMatch).GetINLCTABytes();
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
        Main.LogMessage("Match " + _matchID + " has ended. Notifying players...");
        SendToAll(Packets.MatchEndedPacket());
        _processor.TerminateProcessor();
    }
    public boolean IsPlayerOnTeam(byte idInMatch, byte teamID){
        return _matchTeams.get(teamID).PlayerIDUsed(idInMatch);
    }
    public boolean IsPlayerVerified(byte playerID){
        MatchCharacter toCheck = _matchCharacters.get(playerID);
        if(toCheck != null){
            return toCheck.IsVerified();
        }
        return false;
    }
    public void MarkPlayerVerified(byte playerID, byte teamID){
        Main.LogMessage("MarkPlayerVerified: Fetching player " + playerID);
        Main.LogMessage("MC Size: " + _matchCharacters.size());
        for(MatchCharacter mc : _matchCharacters.values()){
            Main.LogMessage(mc.toString());
        }
        MatchCharacter toVerify = _matchCharacters.get(playerID);
        toVerify.MarkVerified();
        _verifiedClients.put(playerID, toVerify.GetRemoteClient());
        _matchTeams.get(teamID).RegisterVerifiedClient(playerID, toVerify.GetRemoteClient());
    }
    public void SendMatchEndPacket(){
        
    }
    public void SendToAll(byte[] encrypted){
        Main.LogMessage("Sending data to " + _verifiedClients.size() + " clients.");
        _processor.EnqueueForSend(encrypted, _verifiedClients.values());
    }
    public void SendToPlayer(byte[] encrypted, MatchCharacter recipient){
        _processor.EnqueueForSend(encrypted, recipient.GetRemoteClient());
    }
    public void SendToTeam(byte[] encrypted, byte teamID){
        SendToCollection(encrypted, _matchTeams.get(teamID).GetRemoteClients());
    }
    private void SendToCollection(byte[] encrypted, Collection<RemoteClient> recipients){
        _processor.EnqueueForSend(encrypted, recipients);
    }
    public void Tick(long msElapsed){
        CountDownTimedObjects(msElapsed);
    }
    private void CountDownTimedObjects(long msElapsed){
        ArrayList<Byte> expired = new ArrayList<>();
        for(byte key : _objectStatus.keySet()){
            ActivatableObject ao = _objectStatus.get(key);
            if(ao.IsTimed()){
                float timeRemaining =ao.TimeRemaining();
                timeRemaining -= msElapsed;
                if(timeRemaining <= 0){
                    expired.add(key);
                }
                else{
                    ao.SetTimeRemaining(timeRemaining);
                }
            }
            if(!expired.isEmpty()){
                SendToAll(Packets.TimedObjectExpirationPacket(expired));
            }
        }

    }
    public MatchCharacter GetMatchCharacter(byte id){
        return _matchCharacters.get(id);
    }
    public Collection<RemoteClient> GetVerifiedClients(){
        return _verifiedClients.values();
    }
}
