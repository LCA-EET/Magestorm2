import java.util.ArrayList;
import java.util.Collection;
import java.util.concurrent.ConcurrentHashMap;

public class Match {
    protected final byte _matchID;
    protected final int _creatorID;
    protected final byte _sceneID;
    protected final long _expirationTime;
    protected final byte[] _creatorName;
    protected final byte[] _matchBytes;
    protected final byte _lastIndex;
    protected ConcurrentHashMap<Byte, MatchTeam> _matchTeams;
    protected final ConcurrentHashMap<Byte, MatchCharacter> _matchCharacters;
    protected final ConcurrentHashMap<Byte, RemoteClient> _verifiedClients;
    protected ConcurrentHashMap<Byte, ActivatableObject> _objectStatus;
    protected final ConcurrentHashMap<Byte, Integer> _playerScores;
    protected final ConcurrentHashMap<Integer, CastSpell> _castSpells;

    protected byte _nextPlayerID;
    protected final int _matchPort;
    protected InGamePacketProcessor _processor;
    protected final byte _maxPlayers;
    protected byte _matchType;

    private int _nextSpellID = 0;

    protected Match(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte matchType){
        _matchPort = GameServer.GetNextMatchPort();
        _matchType = matchType;
        InitializeActivatables();
        _matchCharacters = new ConcurrentHashMap<>();
        _playerScores = new ConcurrentHashMap<>();
        _castSpells = new ConcurrentHashMap<>();
        _maxPlayers = GameServer.RetrieveMaxPlayerData(sceneID);
        _creatorName = creatorName;
        _nextPlayerID = 1;
        _matchID = matchID;
        _creatorID = creatorID;
        _sceneID = sceneID;
        _expirationTime = creationTime + (3600000 - (duration * 900000)); // 0 = one hour
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
        _matchBytes[index] = matchType;
        index++;
        System.arraycopy(_creatorName, 0, _matchBytes, index, nameBytesLength);
        _verifiedClients = new ConcurrentHashMap<>();
        InitTeams();
    }
    private void InitializeActivatables(){
        _objectStatus = new ConcurrentHashMap<>();
        byte[] objectData = GameServer.GetActivatablesData(_sceneID);
        for(int i = 0; i < objectData.length; i+=2){
            byte objectKey = objectData[i];
            _objectStatus.put(objectKey, new ActivatableObject(this,objectKey, objectData[i+1]));
        }
    }
    protected void AdjustPlayerScore(byte playerID, int adjustment)
    {
        if(!_playerScores.containsKey(playerID)){
            _playerScores.put(playerID, adjustment);
        }
        else{
            int newScore = _playerScores.get(playerID) + adjustment;
            _playerScores.put(playerID, newScore);
        }
    }

    public MatchTeam GetMatchTeam(byte teamID){
        return _matchTeams.get(teamID);
    }
    
    public void ChangeObjectState(byte objectID, byte status){
        if(!_objectStatus.containsKey(objectID)){
            _objectStatus.put(objectID, new ActivatableObject(this, objectID, 5));
            // by default objects will hold their state for 5 seconds. This can be overridden by
            // adding the appropriate entry to the activatables field in the levels table
        }
        _objectStatus.get(objectID).ChangeState(status);
    }

    public void ProcessObjectStatusPacket(byte requesterID){
        ArrayList<Byte> toReturn = new ArrayList<>();
        for(byte objectID : _objectStatus.keySet()){
            byte state = _objectStatus.get(objectID).GetStatus();
            if(state > 0){
                toReturn.add(objectID);
                toReturn.add(state);
            }
        }
        byte[] toSend = Packets.ObjectStatusBytes(toReturn);
        SendToPlayer(toSend, requesterID);
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
        return (byte)_verifiedClients.size();
    }
    public byte[] ToByteArray(){
        _matchBytes[_lastIndex] = NumPlayersInMatch();
        return _matchBytes;
    }
    public boolean HasRoomForAnotherPlayer(){
        return NumPlayersInMatch() < _maxPlayers;
    }
    public byte JoinMatch(RemoteClient rc, byte teamID){
        byte playerID = ObtainNextPlayerID();
        MatchTeam matchTeam = _matchTeams.get(teamID);
        MatchCharacter toAdd = new MatchCharacter(rc.GetActiveCharacter(), teamID, playerID, this);
        matchTeam.AddPlayer(playerID, toAdd);
        _matchCharacters.put(playerID, toAdd);
        Main.LogMessage("Join Match MCSIZE: " + _matchCharacters.size());
        Main.LogMessage("Match " + _matchID +": Added player " + playerID + " to team " + teamID + ", scene: " + _sceneID);
        return playerID;
    }
    public void UpdatePlayerLocation(byte[] decrypted){
        byte playerID = decrypted[1];
        if(_matchCharacters.containsKey(playerID)){
            MatchCharacter toUpdate = _matchCharacters.get(playerID);
            byte controlCode = decrypted[2];
            switch(controlCode){
                case 0: // position only
                    toUpdate.UpdatePosition(decrypted);
                    break;
                case 1: // direction only
                    toUpdate.UpdateDirection(decrypted, 3);
                    break;
                case 2: // position and direction
                    toUpdate.UpdatePosition(decrypted);
                    toUpdate.UpdateDirection(decrypted, 15);
                    break;
            }
            SendToAll(Packets.PlayerLocationBytes(decrypted));
        }
    }
    public void LeaveMatch(byte id, byte team, boolean send){
        _matchCharacters.remove(id).PC().MarkRemovedFromMatch();
        Main.LogMessage("Leave Match MCSIZE: " + _matchCharacters.size());
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

    public void SendPlayerData(byte requestorID, byte idInMatch){
        if(_matchCharacters.containsKey(requestorID)){
            SendToPlayer(Packets.PlayerDataPacket(_matchCharacters.get(idInMatch).GetINLCTABytes()), requestorID);
        }
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
            if(_nextPlayerID > 100){
                _nextPlayerID = 1;
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

    public void SendToPlayer(byte[] encrypted, byte playerID){
        SendToPlayer(encrypted, _matchCharacters.get(playerID));
    }

    protected void SendToCollection(byte[] encrypted, Collection<RemoteClient> recipients){
        Main.LogMessage("Sending data to " + _verifiedClients.size() + " collection.");
        _processor.EnqueueForSend(encrypted, recipients);
    }
    public void Tick(long msElapsed){
        CountDownTimedObjects(msElapsed);
    }
    private void CountDownTimedObjects(long msElapsed){
        for(ActivatableObject ao : _objectStatus.values()){
            if(ao.TimeRemaining() > 0){
                ao.Tick(msElapsed);
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
    public CastSpell GetCastSpell(int id)
    {
        return _castSpells.get(id);
    }
    public MatchCharacter GetMatchCharacter(byte id){
        return _matchCharacters.get(id);
    }
    public Collection<RemoteClient> GetVerifiedClients(){
        return _verifiedClients.values();
    }
    public byte GetSceneID(){
        return _sceneID;
    }

    public void PlayerHit(byte hitterID, byte hitPlayerID, int spellID){
        MatchCharacter hitPlayer = GetMatchCharacter(hitPlayerID);
        if(hitPlayer != null){
            CastSpell spell = GetCastSpell(spellID);
            if(spell != null){
                short damageAmount = ((DamagingSpell)spell).GetDamage();
                hitPlayer.TakeDamage(damageAmount, hitterID);
            }
        }
    }

    public void ProcessSpellCast(byte[] decrypted){
        byte casterID = decrypted[1];
        byte spellID = decrypted[2];
        MatchCharacter caster = _matchCharacters.get(casterID);
        if(caster != null){
            Spell toCast = SpellManager.GetSpell(spellID);
            if(toCast != null){
                byte spellCost = toCast.SpellCost();
                if(caster.GetRemainingMana() >= spellCost){
                    caster.AdjustMana(-spellCost, true);
                    SendToAll(Packets.SpellCastPacket(decrypted));
                }
            }
        }
    }

    public boolean IsCharacterAlive(byte idInMatch){
        MatchCharacter toCheck = _matchCharacters.get(idInMatch);
        if(toCheck != null){
            return toCheck.IsAlive();
        }
        return false;
    }

    protected void PlayerKilled(byte idInMatch, byte damageSource){
        SendToAll(Packets.PlayerKilledPacket(idInMatch, damageSource));
        AdjustPlayerScore(idInMatch, -1);
    }
}
