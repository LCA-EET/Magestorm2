import java.util.ArrayList;
import java.util.HashSet;

public class MatchCharacter extends TimedObject{
    private final MatchTeam _team;
    private final Match _owningMatch;
    private RemoteClient _remote;
    private final PlayerCharacter _pc;

    private final byte _teamID;
    private final float _hpRegenAmount, _spRegenAmount;
    private byte _pmd;

    private final byte[] _INLCTA;
    private final int _idxLevel = 7;
    private final int _idxClass = 8;
    private final int _positionIndex, _directionIndex, _aliveIndex;

    private final long _manaRegenTick;
    private long _manaRegenElapsed;
    private final long _hpRegenTick;
    private long _hpRegenElapsed;
    private final long _waitForHPRegen;
    private long _hpRegenWaitElapsed;
    private final byte[] _playerData;
    private float _currentHP, _currentMana, _maxHP, _maxMana;
    private float _priorHP, _priorMana;
    private float _ley;
    private int _lastPRPacketID;
    private final float[] _resistance;
    private final boolean _newToMatch;
    private final HashSet<Short> _splashHits;
    private byte _wallCount, _sigilCount;
    private byte[] _scoreBytes;
    private final byte _maxWalls, _maxSigils;
    private int _startingXP;
    private float _endingXP, _priorXP;
    private final TimedObjectCollection<Byte, AppliedEffect> _activeEffects;
    private boolean _quitGame;
    public MatchCharacter(PlayerCharacter pc, byte idInMatch, Match match, long hpRegenTick, MatchTeam team, boolean newToMatch){
        _objectID = idInMatch;
        SetDurationRemaining(ServerParams.IngameInactivityDisconnect);
        _team = team;
        _wallCount = 0;
        _sigilCount = 0;
        _resistance = new float[10];
        _lastPRPacketID = 0;
        _hpRegenElapsed = 0;
        _hpRegenTick = hpRegenTick;
        _manaRegenElapsed = 0;
        _manaRegenTick = 1000;
        _waitForHPRegen = 10000;
        boolean joinAlive = match.JoinAlive(team.GetTeamID());
        _newToMatch = newToMatch;
        _pc = pc;
        _startingXP = _pc.GetExperience();
        _endingXP = _startingXP;
        _priorXP = _endingXP;
        _maxWalls = (byte) (3 + Math.floor(pc.GetCharacterLevel() / 3.0f));
        _maxSigils = _maxWalls;
        _maxHP = _pc.GetMaxHP();
        _maxMana = _pc.GetMaxMana();
        _currentHP = joinAlive?_maxHP:0;
        _currentMana = joinAlive?_maxMana:0;
        _owningMatch = match;
        _ley = _pc.GetCharacterClass().GetClassID() == CharacterClass.Mentalist? 0.6f : 0.0f;
        _hpRegenAmount = (1 + (_pc.GetMaxHP() / 25));
        _spRegenAmount = (1 + (_pc.GetMaxMana() / 25));
        _teamID = team.GetTeamID();
        _pc.SetMatchDetails(idInMatch, (byte)match.ObjectID(), _teamID);
        byte[] nameLevelClass = _pc.GetNameLevelClassBytes();
        _INLCTA = new byte[nameLevelClass.length + 7];
        _INLCTA[0] = idInMatch;
        _INLCTA[1] = _teamID;
        byte[] appearanceBytes = pc.GetAppearanceBytes();
        System.arraycopy(appearanceBytes, 0, _INLCTA, 2, appearanceBytes.length);
        System.arraycopy(nameLevelClass, 0, _INLCTA, 7, nameLevelClass.length);
        _playerData = new byte[_INLCTA.length + 17];
        _positionIndex = _INLCTA.length;
        _directionIndex = _positionIndex + 12;
        _aliveIndex = _directionIndex + 4;
        _playerData[_aliveIndex] = (byte)(IsAlive()?1:0);
        _splashHits = new HashSet<>();
        _activeEffects = new TimedObjectCollection<>(1000);
        System.arraycopy(_INLCTA, 0, _playerData, 0, _INLCTA.length);
        InitializeScoreBytes(nameLevelClass);
    }
    public void QuitGame(){
        SetDurationRemaining(0);
        _quitGame = true;
    }
    public boolean PlayerQuit(){
        return _quitGame;
    }
    private void InitializeScoreBytes(byte[] nlc){
        _scoreBytes = new byte[3 + nlc.length];
        System.arraycopy(nlc, 0, _scoreBytes, 3, nlc.length);
    }
    public byte[] GetScoreBytes(){
        return _scoreBytes;
    }
    //region Sigils
    public void IncrementSigilCount() { _sigilCount++; }
    public void DecrementSigilCount() { _sigilCount--; }
    public boolean CanCastAdditionalSigil() {return _sigilCount < _maxSigils; }
    //endregion

    //region Walls
    public void IncrementWallCount()
    {
        _wallCount++;
    }
    public void DecrementWallCount(){
        _wallCount--;
    }
    public boolean CanCastAdditionalWall(){
        return _wallCount < _maxWalls;
    }
    //endregion
    public void RegisterSplashHit(short castID){
        _splashHits.add(castID);
    }
    public void DeregisterSplashHit(short castID){
        _splashHits.remove(castID);
    }
    public boolean IsSplashHit(short castID){
        return _splashHits.contains(castID);
    }
    public byte IsNewToMatch(){
        return _newToMatch?(byte)1:(byte)0;
    }
    public float GetResistance(byte elementID){
        return _resistance[elementID];
    }
    public void AdjustResistance(byte elementID, float resistance){
        _resistance[elementID] += resistance;
    }
    public int GetCharacterID(){
        return _pc.GetCharacterID();
    }
    public int GetLastPRPacketID(){
        return _lastPRPacketID;
    }
    public void SetLey(float ley){
        _ley = ley;
    }
    public void Revive(byte reviverID, float hp){
        _currentHP = hp;
        _owningMatch.SendToAll(Packets.PlayerRevivedPacket((byte)_objectID, reviverID, _currentHP));
    }
    //region Experience
    public void SetExperience(int experience){
        _endingXP = experience;
        _startingXP = -1;
    }
    public void AdjustExperience(float experience){
        _endingXP += experience;
        if(_endingXP < _startingXP){
            _endingXP = _startingXP;
        }
    }
    public void MultiplyExperience(float factor){
        _endingXP = (_endingXP - _startingXP) * factor;
    }
    public float ReportXP(){
        if(_priorXP == _endingXP){
            return 0;
        }
        else{
            _priorXP = _endingXP;
            return _endingXP;
        }
    }
    public int GetStartingXP(){
        return _startingXP;
    }
    public int GetEndingXP(){
        return (int)Math.floor(_endingXP);
    }
    //endregion
    public void SetHP(float newHP){
        _currentHP = newHP;
    }
    public void SetToMaxHP(){
        _currentHP = _maxHP;
    }
    public void TakeDamage(float damageAmount, MatchCharacter attacker){
        _hpRegenWaitElapsed = 0;
        Main.LogMessage("HP pre-adjustment: " + _currentHP);
        _currentHP -= damageAmount;
        Main.LogMessage("HP post-adjustment: " + _currentHP);
        if(_currentHP <= 0){
            _owningMatch.PlayerKilled(this, attacker);
            RemoveAllEffects();

        }
    }
    public byte GetStatistic(byte statCode){
        return _pc.GetStatistic(statCode);
    }
    public void Heal(float healAmount, MatchCharacter healer){
        _hpRegenWaitElapsed = 0;
        _currentHP = Math.min(_currentHP + healAmount, _maxHP);
    }
    public boolean IsAlive(){
        return _currentHP > 0;
    }
    public boolean IsAliveButInjured() {return (_currentHP > 0) && (_currentHP < _maxHP);}
    public boolean HasFullSP(){
        return _currentMana == _maxMana;
    }
    public PlayerCharacter PC(){
        return _pc;
    }

    public CharacterClass GetClass(){
        return _pc.GetCharacterClass();
    }

    public byte GetClassCode(){
        return _pc.GetCharacterClass().GetClassID();
    }

    public byte[] GetINLCTABytes(){
        return _INLCTA;
    }
    public byte[] GetPlayerData(){
        _playerData[_aliveIndex] = (byte)(IsAlive() ? 1 : 0);
        return _playerData;
    }
    public MatchTeam GetTeam(){
        return _team;
    }

    public byte GetTeamID(){
        return _teamID;
    }

    public byte GetIDinMatch(){
        return (byte)_objectID;
    }

    public String GetCharacterName(){
        return _pc.GetCharacterName();
    }

    public void MarkVerified(RemoteClient remote){
        Main.LogMessage("Player " + _objectID + " verified for team " + _teamID);
        SetDurationRemaining(ServerParams.IngameInactivityDisconnect);
        _remote = remote;
    }

    public void AddMana(short amount){
        float newMana = _currentMana + amount;
        _currentMana = Math.min(newMana, _maxMana);
    }

    public RemoteClient GetRemoteClient(){
        return _remote;
    }

    public float GetMaxHP(){
        return _maxHP;
    }

    //region Effects
    public void TerminateEffects(byte[] cancelled){
        for(byte b : cancelled){
            _activeEffects.remove(b);
        }
        _owningMatch.SendToAll(Packets.EffectsCancellationPacket((byte)_objectID, cancelled));
    }
    public boolean IsShocked(){
        return _activeEffects.containsKey(ControlCodes.EffectCode_Shock);
    }
    public void AddEffect(AppliedEffect toAdd){
        byte effectCode = toAdd.GetEffectCode();
        Main.LogDebug("Match " + _owningMatch.ObjectID() + ": applied effect " + effectCode +" to player " + _objectID);
        _activeEffects.put(effectCode, toAdd);
    }
    public void RemoveAllEffects()
    {
        Main.LogDebug("Match " + _owningMatch.ObjectID() + ": All effects removed for player " + _objectID);
        _activeEffects.clear();
    }
    public void CountdownEffects(long msElapsed){
        _activeEffects.CountdownObjects(msElapsed);
    }
    // endregion
    public boolean RegenerateHP(long msElapsed){
        if(_hpRegenWaitElapsed >= _waitForHPRegen){
            if(_hpRegenElapsed >= _hpRegenTick){
                _hpRegenElapsed -= _hpRegenTick;
                if(_currentHP + _hpRegenAmount > _maxHP){
                    _currentHP = _maxHP;
                }
                else{
                    _currentHP += _hpRegenAmount;
                }
            }
            else{
                _hpRegenElapsed += msElapsed;
            }
        }
        else{
            _hpRegenWaitElapsed += msElapsed;
        }
        if(_priorHP != _currentHP){
            _priorHP = _currentHP;
            return true;
        }
        return false;
    }

    public boolean RegenerateSP(long msElapsed){
        _manaRegenElapsed += msElapsed;
        if(_manaRegenElapsed >= _manaRegenTick){
            _manaRegenElapsed -= _manaRegenTick;
            float regenAmount = 1 + (_ley * _spRegenAmount);
            if(_currentMana + regenAmount > _maxMana){
                _currentMana = _maxMana;
            }
            else{
                _currentMana += regenAmount;
            }
        }
        if(_priorMana != _currentMana){
            //Main.LogMessage("Player " + _idInMatch + " mana increased from " + _priorMana + " to " + _currentMana);
            _priorMana = _currentMana;
            return true;
        }
        return false;
    }

    public byte[] GetPosition(){
        byte[] toReturn = new byte[12];
        System.arraycopy(_playerData, _positionIndex, toReturn, 0, 12);
        return toReturn;
    }

    public void UpdateLastMovementPacketID(int packetID, byte pmd){
        _pmd = pmd;
        _lastPRPacketID = packetID;
    }
    protected void UpdatePosition(byte[] decrypted){
        System.arraycopy(decrypted, 8, _playerData, _positionIndex, 12);
    }
    protected void UpdateDirection(byte[] decrypted, int index){
        System.arraycopy(decrypted, index, _playerData, _directionIndex, 4);
    }
    public byte GetSkillLevel(byte disciplineCode){
        return _pc.GetSkillLevel(disciplineCode);
    }
    public float GetCurrentHP(){
        return _currentHP;
    }
    public float GetCurrentMana(){
        return _currentMana;
    }
    public byte GetLevel(){
        return _INLCTA[_idxLevel];
    }
    public short CastSpell(Spell cast, byte[] decrypted){
        short toReturn = -1;
        if(IsAlive()){
            byte spellCost = cast.SpellCost();
            byte skillRequired = cast.GetSkillRequired();
            byte discipline = cast.GetDisciplineCode();
            if(spellCost < _currentMana && skillRequired <= _pc.GetSkillLevel(discipline)){
                toReturn = _owningMatch.SpellCast(this, cast, decrypted); // instantiation happens here
                if(toReturn != -1){
                    _currentMana -= spellCost;
                }
            }
        }
        return toReturn;
    }
    public boolean IsEffectPrevented(byte effectCode){
        for(AppliedEffect ae : _activeEffects.values()){
            if(ae.IsPreventingEffect(effectCode)){
                return true;
            }
        }
        return false;
    }
    @Override
    public String toString(){
        return "MCID: " + _objectID + ", TeamID: " + _teamID + ", RC: " + _remote.toString();
    }

}
