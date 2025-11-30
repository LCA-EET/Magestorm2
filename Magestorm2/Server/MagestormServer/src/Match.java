import java.util.ArrayList;
import java.util.Collection;
import java.util.concurrent.ConcurrentHashMap;

public class Match {
    protected final byte _matchID;
    protected final int _creatorID;
    protected final byte _sceneID;
    protected final long _expirationTime;
    protected final long _regenTick;

    protected final byte[] _creatorName;
    protected final byte[] _matchBytes;
    protected final MatchOptions _matchOptions;
    protected final byte _lastIndex;
    protected ConcurrentHashMap<Byte, MatchTeam> _matchTeams;
    protected final ConcurrentHashMap<Byte, MatchCharacter> _matchCharacters;
    protected final ConcurrentHashMap<Integer, MatchCharacter> _unverifiedCharacters;
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

    protected Match(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte matchType, byte[] matchOptions){
        _matchOptions = new MatchOptions(matchOptions);
        byte matchOptionsLength = (byte)matchOptions.length;
        _regenTick = _matchOptions.IsOptionSet(MatchOptions.FastRegen)?1000:5000;
        _matchPort = GameServer.GetNextMatchPort();
        _matchType = matchType;
        _sceneID = sceneID;
        _matchCharacters = new ConcurrentHashMap<>();
        _unverifiedCharacters = new ConcurrentHashMap<>();
        _playerScores = new ConcurrentHashMap<>();
        _castSpells = new ConcurrentHashMap<>();
        _maxPlayers = GameServer.RetrieveMaxPlayerData(sceneID);
        _creatorName = creatorName;
        _nextPlayerID = 1;
        _matchID = matchID;
        _creatorID = creatorID;
        _expirationTime = creationTime + (3600000 - (duration * 900000)); // 0 = one hour
        LogMessage("Initializing match " + _matchID + " with expiration time: " + _expirationTime);
        byte nameBytesLength = (byte)_creatorName.length;
        _matchBytes = new byte[1 + 1 + 8 + 4 + 1 + 1 + 1 + matchOptions.length + nameBytesLength + 1];
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
        _matchBytes[index] = matchOptionsLength;
        index++;
        System.arraycopy(matchOptions, 0, _matchBytes, index, matchOptionsLength);
        index+= matchOptionsLength;
        System.arraycopy(_creatorName, 0, _matchBytes, index, nameBytesLength);
        _verifiedClients = new ConcurrentHashMap<>();
        InitTeams();
        InitializeActivatables();
    }
    public boolean IsOptionEnabled(int optionCode){
        return _matchOptions.IsOptionSet(optionCode);
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
    
    public void ChangeObjectState(byte objectID, byte status, byte changedBy, byte selfReset){
        if(selfReset > 0){
            // this is a self-resetting object. There is no need for the server to send a state reset, so just forward the packet.
            SendToAll(Packets.ObjectStateChangePacket(objectID, status));
        }
        else{
            if(!_objectStatus.containsKey(objectID)){
                _objectStatus.put(objectID, new ActivatableObject(this, objectID, 0));
                // by default objects will hold their state indefinitely. This can be overridden by
                // adding the appropriate entry to the activatables field in the levels table
            }
            ActivatableObject toChange = _objectStatus.get(objectID);
            if(toChange.GetStatus() != status){
                _objectStatus.get(objectID).ChangeState(status);
                SendToAll(Packets.ObjectStateChangePacket(objectID, status));
            }
            else{ // this player is out-of-sync with the server.
                SendToPlayer(Packets.ObjectStateChangePacket(objectID, status), changedBy);
            }
        }

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
        LogMessage("Teams initialized.");
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
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID){
        byte playerID = ObtainNextPlayerID();
        MatchCharacter toAdd = new MatchCharacter(rc.GetActiveCharacter(), teamID, playerID, this, _regenTick);
        _unverifiedCharacters.put(rc.AccountID(), toAdd);
        LogMessage("Added player " + playerID + " to team " + teamID + ", scene: " + _sceneID);
        return toAdd;
    }
    public boolean IsAwaitingVerification(int accountID){
        return _unverifiedCharacters.containsKey(accountID);
    }
    public void UpdatePlayerLocation(byte[] decrypted){
        byte playerID = decrypted[1];
        if(_matchCharacters.containsKey(playerID)){
            MatchCharacter toUpdate = _matchCharacters.get(playerID);
            int packetID = ByteUtils.ExtractInt(decrypted, 2);
            if(packetID > toUpdate.GetLastPRPacketID()){
                byte controlCode = decrypted[6];
                switch(controlCode){
                    case 0: // position only
                        toUpdate.UpdatePosition(decrypted);
                        break;
                    case 1: // direction only
                        toUpdate.UpdateDirection(decrypted, 7);
                        break;
                    case 2: // position and direction
                        toUpdate.UpdatePosition(decrypted);
                        toUpdate.UpdateDirection(decrypted, 19);
                        break;
                }
                SendToAll(Packets.PlayerMovedPacket(decrypted));
            }
        }
    }
    public void UpdatePlayerLey(byte[] decrypted){
        byte playerID = decrypted[1];
        float newLey = ByteUtils.ExtractFloat(decrypted, 2);
        if(newLey < 0.0f || newLey > 1.0f){
            LogMessage("Invalid ley: " + newLey + " for player " + playerID);
        }
        else{
            MatchCharacter mc = _matchCharacters.get(playerID);
            mc.SetLey(newLey);
            SendToPlayer(Packets.HPorManaorLeyUpdatePacket(InGame_Send.LeyUpdate, newLey), mc);
        }
    }
    public void LeaveMatch(byte id, byte team, boolean send){
        _matchCharacters.remove(id).PC().MarkRemovedFromMatch();
        LogMessage("Player " + id + " has left the match. Players remaining: " + _matchCharacters.size());
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
        LogMessage("The match has ended. Notifying players...");
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
            LogMessage("toCheck in IsPlayerVerified is null, for player: " + playerID);
        }
        return false;
    }
    public void MarkPlayerVerified(byte playerID, byte teamID, int accountID){
        LogMessage("MarkPlayerVerified: Fetching player " + playerID);
        MatchCharacter toVerify = _unverifiedCharacters.get(accountID);
        if(toVerify != null){
            toVerify.MarkVerified();
            _matchCharacters.put(playerID, toVerify);
            MatchTeam team = _matchTeams.get(teamID);
            _unverifiedCharacters.remove(accountID);
            _verifiedClients.put(playerID, toVerify.GetRemoteClient());
            team.AddPlayer(playerID, toVerify);
            team.RegisterVerifiedClient(playerID, toVerify.GetRemoteClient());
        }

    }
    public void SendMatchEndPacket(){
        
    }
    public void SendToAll(byte[] encrypted){
        _processor.EnqueueForSend(encrypted, _verifiedClients.values());
    }
    public void SendToPlayer(byte[] encrypted, MatchCharacter recipient){
        _processor.EnqueueForSend(encrypted, recipient.GetRemoteClient());
    }

    public void SendToPlayer(byte[] encrypted, byte playerID){
        SendToPlayer(encrypted, _matchCharacters.get(playerID));
    }

    protected void SendToCollection(byte[] encrypted, Collection<RemoteClient> recipients){
        _processor.EnqueueForSend(encrypted, recipients);
    }
    public void Tick(long msElapsed){
        CountDownTimedObjects(msElapsed);
        RegeneratePlayerHM(msElapsed);
    }
    private void RegeneratePlayerHM(long msElapsed){
        for(MatchCharacter mc : _matchCharacters.values()){
            boolean hpChanged = false;
            boolean manaChanged = false;
            if(mc.IsAliveButInjured()){
                hpChanged = mc.RegenerateHP(msElapsed);
            }
            if(mc.IsAlive() && !mc.HasFullSP()){
                manaChanged = mc.RegenerateSP(msElapsed);
            }
            if(!hpChanged && !manaChanged){
                return;
            }
            else{
                if(hpChanged && manaChanged){
                    SendToPlayer(Packets.HPandManaUpdatePacket(mc.GetCurrentHP(), mc.GetCurrentMana()), mc);
                }
                else if (hpChanged) {
                    SendToPlayer(Packets.HPorManaorLeyUpdatePacket(InGame_Send.HPUpdate, mc.GetCurrentHP()), mc);
                }
                else{
                    SendToPlayer(Packets.HPorManaorLeyUpdatePacket(InGame_Send.ManaUpdate, mc.GetCurrentMana()), mc);
                }
            }
        }
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
                LogMessage("Sending inactivity termination.");
                _inactiveClients.add(mc.GetRemoteClient());
                _departedCharacters.add(mc);
            }
            else if (mc.InactivityExceededWarningThreshold()){
                _warningClients.add(mc.GetRemoteClient());
            }
        }
        if(!_warningClients.isEmpty()){
            LogMessage("Sending inactivity warning.");
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
                    caster.AdjustMana(-spellCost);
                    SendToAll(Packets.SpellCastPacket(decrypted));
                }
            }
        }
    }

    public boolean IsCharacterAlive(byte idInMatch){
        if(idInMatch == 0){
            return true;
        }
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

    public byte GetMatchType(){
        return _matchType;
    }

    public boolean ParseCommand(String command, String[] params, byte senderID){
        Main.LogMessage("Command: " + command);
        switch(command){
            case "killself":
                _matchCharacters.get(senderID).TakeDamage((short)30000, senderID);
                return true;
            case "o":
                SendTeamMessage(params, " ", 1, senderID, MatchTeam.Order);
                return true;
            case "c":
                SendTeamMessage(params, " ", 1, senderID, MatchTeam.Chaos);
                return true;
            case "b":
                SendTeamMessage(params, " ", 1, senderID, MatchTeam.Balance);
                return true;
        }
        return false;
    }
    private void SendTeamMessage(String[] params, String delimeter, int startIndex, byte senderID, byte teamID){
        byte[] messageBytes = ByteUtils.UTF8toBytes(params, delimeter, startIndex);
        SendToCollection(Packets.TeamChatPacket(messageBytes, senderID, teamID),
                _matchTeams.get(teamID).GetRemoteClients());
        MatchCharacter sender = GetMatchCharacter(senderID);
        if(sender != null){
            if(sender.GetTeamID() != teamID){
                SendToPlayer(Packets.TeamChatPacket(messageBytes, senderID, teamID), sender);
            }
        }
    }

    public void LogMessage(String toLog){
        Main.LogMessage("Match " + _matchID +": " + toLog);
    }
    public void LogError(String toLog){
        Main.LogError("Match " + _matchID + ": " + toLog);
    }
}
