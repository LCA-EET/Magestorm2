import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;
import java.util.concurrent.ConcurrentHashMap;

public class Match {
    protected final byte _matchID;
    protected final int _creatorID;
    protected final byte _sceneID;
    protected final long _expirationTime;
    protected final long _regenTick;
    protected final byte[] _creatorName;
    protected final byte[] _matchBytes;
    protected byte[] _scoreBytes;
    protected final MatchOptions _matchOptions;
    protected final byte _lastIndex;
    protected ConcurrentHashMap<Byte, MatchTeam> _matchTeams;
    protected final ConcurrentHashMap<Byte, MatchCharacter> _matchCharacters;
    protected final ConcurrentHashMap<Integer, MatchCharacter> _unverifiedCharacters;
    protected final ConcurrentHashMap<Byte, RemoteClient> _verifiedClients;
    protected ConcurrentHashMap<Byte, ActivatableObject> _objectStatus;
    protected final ConcurrentHashMap<Short, CastSpell> _castSpells;
    protected final ConcurrentHashMap<Integer, Score> _playerScores;
    protected final ConcurrentHashMap<Short, Wall> _walls;
    protected final HashSet<Integer> _playersJoined;
    protected byte _nextPlayerID;
    protected final int _matchPort;
    protected InGamePacketProcessor _processor;
    protected final byte _maxPlayers;
    protected byte _matchType;
    protected short _nextCastID = 0;
    protected boolean _scoreUpdated = false;
    private long _spellExpirationElapsed = 0;
    private long _expCheckElapsed = 0;
    private final long _expReportInterval = 30000;
    protected Match(byte matchID, int creatorID, byte[] creatorName, byte sceneID, long creationTime, byte duration, byte matchType, byte matchOptions){
        _playersJoined = new HashSet<>();
        _matchOptions = new MatchOptions(matchOptions);
        _regenTick = _matchOptions.IsOptionSet(ControlCodes.MatchOptions_FastRegen)?1000:5000;
        _matchPort = GameServer.GetNextMatchPort();
        _matchType = matchType;
        _sceneID = sceneID;
        _castSpells = new ConcurrentHashMap<>();
        _walls = new ConcurrentHashMap<>();
        _matchCharacters = new ConcurrentHashMap<>();
        _unverifiedCharacters = new ConcurrentHashMap<>();
        _playerScores = new ConcurrentHashMap<>();
        _maxPlayers = GameServer.RetrieveMaxPlayerData(sceneID);
        _creatorName = creatorName;
        _nextPlayerID = 1;
        _matchID = matchID;
        _creatorID = creatorID;
        _expirationTime = creationTime + (3600000 - (duration * 900000)); // 0 = one hour
        //_expirationTime = creationTime+30000;
        LogMessage("Initializing match " + _matchID + " with expiration time: " + _expirationTime + " on port " + _matchPort);
        byte nameBytesLength = (byte)_creatorName.length;
        _matchBytes = new byte[1 + 1 + 8 + 4 + 1 + 1 + 1 + nameBytesLength + 1];
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
        _matchBytes[index] = matchOptions;
        index++;
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
    
    public void IncrementPlayerDeaths(MatchCharacter mc){
        int characterID = mc.GetCharacterID();
        if(!_playerScores.containsKey(characterID)){
            _playerScores.put(characterID, new Score(mc));
        }
        _playerScores.get(characterID).IncrementDeaths();
    }
    public void IncrementPlayerKills(MatchCharacter mc){
        int characterID = mc.GetCharacterID();
        if(!_playerScores.containsKey(characterID)){
            _playerScores.put(characterID, new Score(mc));
        }
        _playerScores.get(characterID).IncrementKills();
    }
    public void IncrementPlayerRaises(MatchCharacter mc){
        int characterID = mc.GetCharacterID();
        if(!_playerScores.containsKey(characterID)){
            _playerScores.put(characterID, new Score(mc));
        }
        _playerScores.get(characterID).IncrementRaises();
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
    //region Scoring
    public void RefreshScoreBytes(int topX){
        ArrayList<byte[]> scoreBytes = new ArrayList<>();
        int arrayLength = 2;
        ArrayList<Score> topXscores = new ArrayList<>(_playerScores.values());
        Collections.sort(topXscores);
        for(int i = 0; i < topXscores.size(); i++){
            Score score = topXscores.get(i);
            byte[] playerScoreBytes = score.GetScorer().GetScoreBytes();
            playerScoreBytes[0] = score.GetKills();
            playerScoreBytes[1] = score.GetDeaths();
            playerScoreBytes[2] = score.GetRaises();
            arrayLength+= playerScoreBytes.length;
            scoreBytes.add(playerScoreBytes);
            if(i > topX){
                break;
            }
        }
        _scoreBytes = new byte[arrayLength];
        int index = 2;
        for(byte[] playerScoreBytes : scoreBytes){
            System.arraycopy(playerScoreBytes, 0, _scoreBytes, index, playerScoreBytes.length);
            index+=playerScoreBytes.length;
        }
        _scoreBytes[0] = InGame_Send.MatchScores;
        _scoreBytes[1] = (byte)scoreBytes.size();
    }
    public byte[] GetScoreBytes(){
        return _scoreBytes;
    }
    //endregion
    public boolean HasRoomForAnotherPlayer(){
        return NumPlayersInMatch() < _maxPlayers;
    }
    public MatchCharacter JoinMatch(RemoteClient rc, byte teamID){
        byte playerID = ObtainNextPlayerID();
        PlayerCharacter joining = GameServer.GetActiveCharacter(rc.AccountID());
        int characterID = joining.GetCharacterID();
        boolean newToMatch;
        if(_playersJoined.contains(characterID)){
            newToMatch = false;
        }
        else{
            newToMatch = true;
            _playersJoined.add(characterID);
        }
        MatchCharacter toAdd = new MatchCharacter(joining, playerID, this,
                _regenTick, _matchTeams.get(teamID), newToMatch);
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
                byte pmd = decrypted[6];
                toUpdate.UpdateLastMovementPacketID(packetID, pmd);
                byte controlCode = decrypted[7];
                switch(controlCode){
                    case 0: // position only
                        toUpdate.UpdatePosition(decrypted);
                        break;
                    case 1: // direction only
                        toUpdate.UpdateDirection(decrypted, 8);
                        break;
                    case 2: // position and direction
                        toUpdate.UpdatePosition(decrypted);
                        toUpdate.UpdateDirection(decrypted, 20);
                        break;
                }
                byte[] toEncrypt = new byte[controlCode==2 ? 32 : 20];
                System.arraycopy(decrypted, 0, toEncrypt, 0, toEncrypt.length);
                SendToAll(Cryptographer.Encrypt(toEncrypt));
            }
        }
    }
    public boolean JoinAlive(byte teamID){
        return true;
    }
    public void PlayerTapped(byte playerID){
        MatchCharacter mc = _matchCharacters.get(playerID);
        LogMessage("DM tap: " + playerID + "." );
        if(!mc.IsAlive()){
            mc.SetToMaxHP();
            SendToAll(Packets.PlayerTapped(playerID));
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
    private void RemoveAllPlayers(){
        ArrayList<Byte> playerIDs = new ArrayList<>();
        for (MatchCharacter matchCharacter : _matchCharacters.values()){
            playerIDs.add(matchCharacter.GetIDinMatch());
        }
        for (byte id : playerIDs){
            LeaveMatch(id, false, false);
        }
    }
    public boolean ScoreUpdated(){
        boolean toReturn = _scoreUpdated;
        if(toReturn){
            _scoreUpdated = false;
        }
        return toReturn;
    }
    public void LeaveMatch(byte id, boolean send, boolean quitGame){
        MatchCharacter departee = _matchCharacters.remove(id);
        if(departee != null){
            PlayerCharacter pc = departee.PC();
            _verifiedClients.remove(id);
            _matchTeams.get(departee.GetTeamID()).RemovePlayer(id);
            if(quitGame){
                GameServer.ClientLoggedOut(pc.GetAccountID());
            }
            else{
                RemoteClient rc = GameServer.GetClient(pc.GetAccountID());
                if(rc != null){
                    rc.MarkPortSwitchPending();
                    rc.SetDepartingCharacterID(departee.GetCharacterID());
                    pc.MarkRemovedFromMatch();
                }
            }
            Main.ExperienceUpdater.AddToQueue(departee);
        }
        LogMessage("Player " + id + " has left the match. Players remaining: " + _matchCharacters.size());
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
    public void SendPlayerData(byte requesterID, byte idInMatch){
        if(_matchCharacters.containsKey(requesterID)){
            MatchCharacter subject = _matchCharacters.get(idInMatch);
            if(subject != null){
                byte alive = subject.IsAlive()?(byte)1:(byte)0;
                SendToPlayer(Packets.PlayerDataPacket(subject.GetPlayerData(), alive, subject.IsNewToMatch()), requesterID);
            }

        }
    }
    public int GetMatchPort(){
        return _matchPort;
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
        ArrayList<RemoteClient> remainingClients = new ArrayList<>(_verifiedClients.values());
        SendToCollection(Packets.MatchEndedPacket(), remainingClients);
        RemoveAllPlayers();
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
    public void MarkPlayerVerified(byte playerID, byte teamID, int accountID, RemoteClient remote){
        LogMessage("MarkPlayerVerified: Fetching player " + playerID);
        MatchCharacter toVerify = _unverifiedCharacters.get(accountID);
        if(toVerify != null){
            toVerify.MarkVerified(remote);
            _matchCharacters.put(playerID, toVerify);
            MatchTeam team = _matchTeams.get(teamID);
            _unverifiedCharacters.remove(accountID);
            _verifiedClients.put(playerID, toVerify.GetRemoteClient());
            team.AddPlayer(playerID, toVerify);
            team.RegisterVerifiedClient(playerID, toVerify.GetRemoteClient());
        }

    }
    public short SpellCast(MatchCharacter caster, Spell spellReference, byte[] decrypted){
        short castID = IncrementCastID();
        switch(spellReference.SpellType()){
            case ControlCodes.SpellTypes_Projectile:
            case ControlCodes.SpellTypes_PBAoE:
                if(spellReference.IsDamaging()){
                    _castSpells.put(castID, new DamagingSpell(caster, castID, spellReference, this));
                }
                else if(spellReference.IsHealing()){
                    _castSpells.put(castID, new HealingSpell(caster, castID, spellReference, this));
                }
                break;
            case ControlCodes.SpellTypes_SelfHeal:
                HealingSpell selfHeal = new HealingSpell(caster, castID, spellReference, this);
                selfHeal.ProcessSpell(caster);
                break;
            case ControlCodes.SpellTypes_SelfResist:
                ResistanceSpell resistanceSpell = new ResistanceSpell(caster, castID, spellReference, this);
                resistanceSpell.ProcessSpell(caster);
                break;
            case ControlCodes.SpellTypes_Resistable:
            case ControlCodes.SpellTypes_Bolt:
                _castSpells.put(castID, new DamagingSpell(caster, castID, spellReference, this));
                break;
            case ControlCodes.SpellTypes_Self:
                CastSpell selfCast = new CastSpell(caster, castID, spellReference, this);
                selfCast.ProcessSpell(caster);
                break;
            case ControlCodes.SpellTypes_Summon:

                break;
            case ControlCodes.SpellTypes_SolidWall:
            case ControlCodes.SpellTypes_NonSolidWall:
                if(caster.CanCastAdditionalWall()){
                    byte[] prBytes = new byte[24];
                    System.arraycopy(decrypted, ControlCodes.CastPayloadStartIndex, prBytes, 0, 24);
                    Wall wall = new Wall(caster, castID, spellReference, this, prBytes);
                    if(wall.IsSolidWall() && _matchOptions.IsOptionSet(ControlCodes.MatchOptions_NoSolidWalls)){
                        castID = -1;
                    }
                    else{
                        _castSpells.put(castID, wall);
                        _walls.put(castID, wall);
                    }
                }
                else{
                    castID = -1;
                }
                break;
        }
        return castID;
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
    public void SendToClient(byte[] encrypted, RemoteClient remote){
        _processor.EnqueueForSend(encrypted, remote);
    }
    protected void SendToCollection(byte[] encrypted, Collection<RemoteClient> recipients){
        _processor.EnqueueForSend(encrypted, recipients);
    }
    public void RequestWallData(MatchCharacter requester){
        if(!_walls.isEmpty()){
            ArrayList<Wall> wallData = new ArrayList<>();
            for(Wall wall : _walls.values()){
                wallData.add(wall);
                if(wallData.size() == 19){
                    SendToPlayer(Packets.WallDataPacket(wallData), requester);
                    wallData.clear();
                }
            }
            if(!wallData.isEmpty()){
                SendToPlayer(Packets.WallDataPacket(wallData), requester);
            }
        }
    }
    public void Tick(long msElapsed){
        CountDownTimedObjects(msElapsed);
        PlayerTick(msElapsed);
        ClearExpiredSpells(msElapsed);
        CountdownWalls(msElapsed);
        ExpTick(msElapsed);
    }
    private void ExpTick(long msElapsed){
        _expCheckElapsed += msElapsed;
        if(_expCheckElapsed >= _expReportInterval){
            _expCheckElapsed = 0;
            for(MatchCharacter mc : _matchCharacters.values()){
                float experience = mc.ReportXP();
                if(experience != 0){
                    SendToPlayer(Packets.ExperienceUpdatePacket(experience), mc);
                }
            }
        }
    }
    private void CountdownWalls(long msElapsed){
        if(!_walls.isEmpty()){
            ArrayList<Short> expiredWalls = new ArrayList<>();
            for(Wall wall : _walls.values()){
                if(wall.ReduceDuration(msElapsed)){
                    expiredWalls.add(wall.CastID());
                }
            }
            for(Short wallID : expiredWalls){
                _walls.remove(wallID);
            }
            if(!expiredWalls.isEmpty()){
                SendToAll(Packets.WallsExpirationPacket(expiredWalls));
            }
        }
    }
    public Wall GetWall(short wallID){
        return _walls.get(wallID);
    }
    private void ClearExpiredSpells(long elapsed){
        _spellExpirationElapsed += elapsed;
        if(_spellExpirationElapsed >= 60000){
            _spellExpirationElapsed = 0;
            ArrayList<Short> expiredSpells = new ArrayList<>();
            long currentTimeMillis= System.currentTimeMillis();
            for(CastSpell spell : _castSpells.values()){
                if(spell.IsExpired(currentTimeMillis)){
                    expiredSpells.add(spell.CastID());
                }
            }
            if(!expiredSpells.isEmpty()){
                for(short spellID : expiredSpells){
                    _castSpells.remove(spellID);
                }
            }
        }
    }
    private short IncrementCastID(){
        _nextCastID++;
        return _nextCastID;
    }
    private void PlayerTick(long msElapsed){
        for(MatchCharacter mc : _matchCharacters.values()){
            mc.CountdownEffects(msElapsed);
            boolean hpChanged = false;
            boolean manaChanged = false;
            if(mc.IsAliveButInjured()){
                hpChanged = mc.RegenerateHP(msElapsed);
            }
            if(mc.IsAlive() && !mc.HasFullSP()){
                manaChanged = mc.RegenerateSP(msElapsed);
            }
            if(hpChanged || manaChanged){
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
                LogMessage("Sending inactivity warning.");
                _warningClients.add(mc.GetRemoteClient());
            }
        }
        if(!_warningClients.isEmpty()){
            SendToCollection(Packets.InactivityWarningPacket(), _warningClients);
        }
        if(!_inactiveClients.isEmpty()){
            SendToCollection(Packets.IGInactivityDisconnectPacket(), _inactiveClients);
            for(MatchCharacter mc : _departedCharacters){
                LeaveMatch(mc.GetIDinMatch(), false, true);
            }
            SendToAll(Packets.PlayersLeftMatchPacket(_departedCharacters));
        }

    }
    public CastSpell GetCastSpell(short id)
    {
        return _castSpells.get(id);
    }
    public MatchCharacter GetMatchCharacter(byte id){
        return _matchCharacters.get(id);
    }
    public RemoteClient GetVerifiedClient(byte idInMatch){
        return _verifiedClients.get(idInMatch);
    }
    public Collection<RemoteClient> GetVerifiedClients(){
        return _verifiedClients.values();
    }
    public byte GetSceneID(){
        return _sceneID;
    }

    public void PlayerHit(MatchCharacter hitPlayer, short castID){
        CastSpell spell = GetCastSpell(castID);
        if(spell != null){
            spell.ProcessSpell(hitPlayer);
            if(spell.GetBaseSpell().IsDamaging()){
                byte[] packet = Packets.HitNotificationPacket(hitPlayer.GetIDinMatch(), spell.GetCasterID());
                SendToPlayer(packet, hitPlayer);
            }
        }
        else{
            Main.LogError("Match.PlayerHit: Spell " + castID + " is null.");
        }
    }
    public void PlayerHit(byte hitPlayerID, short castID){
        MatchCharacter hitPlayer = GetMatchCharacter(hitPlayerID);
        if(hitPlayer != null){
            PlayerHit(hitPlayer, castID);
        }
        else{
            Main.LogError("Match.PlayerHit: Player " + hitPlayerID + " is null.");
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

    protected void PlayerKilled(MatchCharacter killed, MatchCharacter killer){
        SendToAll(Packets.PlayerKilledPacket(killed.GetIDinMatch(), killer.GetIDinMatch()));
        IncrementPlayerKills(killer);
        IncrementPlayerDeaths(killed);
        killed.GetTeam().GetScore().IncrementDeaths();
        killer.GetTeam().GetScore().IncrementKills();
        killer.AdjustExperience(killed.GetMaxHP() * (int)(1 + Math.floor(killed.GetLevel() / 8.0f)));
        RefreshScoreBytes(10);
        _scoreUpdated = true;
    }

    public byte GetMatchType(){
        return _matchType;
    }

    public void SendPlayerToValhalla(MatchCharacter player){
        SendToPlayer(Packets.SendToValhallaPacket(), player);
        if(!player.IsAlive()){
            player.Heal(player.GetMaxHP(), null);
        }
    }

    public boolean ParseCommand(String command, String[] params, byte senderID){
        Main.LogMessage("Command: " + command);
        MatchCharacter sender = _matchCharacters.get(senderID);
        switch(command){
            case "setexp":
                sender.SetExperience(Integer.parseInt(params[1]));
                return true;
            case "adjustexp":
                sender.AdjustExperience(Float.parseFloat(params[1]));
                return true;
            case "1hp":
                sender.TakeDamage(sender.GetCurrentHP() - 1, sender);
                return true;
            case "revive":
                sender.Revive(senderID, 1);
                return true;
            case "killself":
                sender.TakeDamage((short)30000, sender);
                return true;
            case "effect":
                byte effectCode = Byte.parseByte(params[1]);
                byte duration = Byte.parseByte(params[2]);
                byte degree = Byte.parseByte(params[3]);
                SendToAll(Packets.ApplyEffectPacket(senderID, senderID, effectCode, duration, degree));
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
    public void HandleBanish(byte[] decrypted){
        byte casterID = decrypted[1];
        MatchCharacter caster = GetMatchCharacter(casterID);
        if(caster != null){
            if(caster.IsAlive()){
                MatchCharacter devoured = GetMatchCharacter(decrypted[2]);
                if(devoured != null){
                    if(!devoured.IsAlive()){
                        byte skillLevel = caster.GetSkillLevel(ControlCodes.Discipline_SpiritLaw);
                        short manaRecovered = 0;
                        if(skillLevel == 2){
                            manaRecovered = devoured.GetLevel();
                        }
                        else if (skillLevel == 3){
                            manaRecovered = (short) (devoured.GetLevel() * 2);
                        }
                        caster.AddMana(manaRecovered);
                        SendPlayerToValhalla(devoured);
                        SendToPlayer(Packets.HPandManaUpdatePacket(devoured.GetCurrentHP(), devoured.GetCurrentMana()), devoured);
                        caster.AdjustExperience(manaRecovered * 2);
                    }
                }
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
