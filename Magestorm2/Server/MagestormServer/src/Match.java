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
    private ConcurrentHashMap<Byte, Pool> _matchPools;
    private byte _nextPlayerID;
    private final int _matchPort;
    private final InGamePacketProcessor _processor;
    private final byte _maxPlayers;
    private final byte _matchType;


    public Match(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte matchType){
        _matchType = matchType;
        _objectStatus = new ConcurrentHashMap<>();
        _matchCharacters = new ConcurrentHashMap<>();
        _maxPlayers = GameServer.RetrieveMaxPlayerData(sceneID);
        _creatorName = creatorName;
        _nextPlayerID = 0;
        _matchID = matchID;
        _creatorID = creatorID;
        _sceneID = sceneID;
        InitializePools();
        _expirationTime = creationTime + (3600000 - (duration * 900000)); // 0 = one hour
        _matchPort = GameServer.GetNextMatchPort();
        Main.LogMessage("Initializing match " + _matchID + " with expiration time: " + _expirationTime);
        byte nameBytesLength = (byte)_creatorName.length;
        _matchBytes = new byte[1 + 1 + 8 + 4 + 1 + 1 + nameBytesLength + 1];
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
        _matchBytes[index] = _matchType;
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
    private void InitializePools(){
        _matchPools= new ConcurrentHashMap<>();
        byte[] poolBytes = GameServer.GetPoolData(_sceneID);
        for(int i = 0; i < poolBytes.length; i+=2){
            byte poolID = poolBytes[i];
            byte poolPower = poolBytes[i+1];
            Pool toAdd = new Pool(this, poolID, poolPower);
            _matchPools.put(poolID, toAdd);
        }
    }

    public void BiasPool(byte biaserID, byte poolID, RemoteClient rc) {
        if(_matchPools.containsKey(poolID)){
            MatchCharacter biaser = _matchCharacters.get(biaserID);
            short diceRoll = GameUtils.DiceRoll(100, 1);
            if(Pool.BiasChance(biaser.GetClassCode()) >= diceRoll){
                _matchPools.get(poolID).Bias(_matchCharacters.get(biaserID));
            }
            else{
                SendToPlayer(Packets.PoolBiasFailurePacket(), biaser);
            }
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
        MatchCharacter toAdd = new MatchCharacter(rc.GetActiveCharacter(), teamID, playerID, this);
        matchTeam.AddPlayer(playerID, toAdd);
        _matchCharacters.put(playerID, toAdd);

        Main.LogMessage("Match " + _matchID +": Added player " + playerID + " to team " + teamID);
        GameServer.EnqueueForSend(Packets.MatchEntryPacket(_sceneID, teamID, playerID, _matchPort, _matchID), rc);
    }
    public void LeaveMatch(byte id, byte team, boolean send){
        _matchCharacters.remove(id).PC().MarkRemovedFromMatch();
        _verifiedClients.remove(id);
        _matchTeams.get(team).RemovePlayer(id);
        if(send){
            SendToAll(Packets.PlayerLeftMatchPacket(id));
        }
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
            toCheck.MarkPacketReceived();
            return toCheck.IsVerified();
        }
        else{
            Main.LogMessage("toCheck in IsPlayerVerified is null, for player: " + playerID);
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
        Main.LogMessage("Sending data to " + _verifiedClients.size() + " collection.");
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
    public void CheckForInactivity(){
        ArrayList<RemoteClient> _inactiveClients = new ArrayList<>();
        ArrayList<MatchCharacter> _departedCharacters = new ArrayList<>();
        ArrayList<RemoteClient> _warningClients = new ArrayList<>();
        for(MatchCharacter mc: _matchCharacters.values()){
            if(mc.InactivityExceededMaximumThreshold()){
                Main.LogMessage("Sending inactivity termination.");
                _inactiveClients.add(mc.GetRemoteClient());
                _departedCharacters.add(mc);
            }
            else if (mc.InactivityExceededWarningThreshold()){
                _warningClients.add(mc.GetRemoteClient());
            }
        }
        if(!_warningClients.isEmpty()){
            Main.LogMessage("Sending inactivity warning.");
            SendToCollection(Packets.InactivityWarningPacket(), _warningClients);
        }
        if(!_inactiveClients.isEmpty()){
            SendToCollection(Packets.InactivityDisconnectPacket(), _inactiveClients);
            for(MatchCharacter mc : _departedCharacters){
                LeaveMatch(mc.GetIDinMatch(), mc.GetTeamID(), false);
            }
            SendToAll(Packets.PlayersLeftMatchPacket(_departedCharacters));
        }

    }
    public MatchCharacter GetMatchCharacter(byte id){
        return _matchCharacters.get(id);
    }
    public Collection<RemoteClient> GetVerifiedClients(){
        return _verifiedClients.values();
    }

    public byte[] GetPoolBiasData(){
        byte[] toReturn = new byte[1 + (_matchPools.size() * 3)];
        toReturn[0] = (byte)_matchPools.size();
        int trIndex = 1;
        for(Pool pool : _matchPools.values() ){
            toReturn[trIndex] = pool.GetPoolID();
            toReturn[trIndex + 1] = pool.GetPoolTeam();
            toReturn[trIndex + 2] = pool.GetPoolBiasAmount();
            trIndex += 3;
        }
        return toReturn;
    }
}
