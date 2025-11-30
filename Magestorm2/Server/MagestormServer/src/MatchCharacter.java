public class MatchCharacter {
    private final Match _owningMatch;

    private final PlayerCharacter _pc;

    private final byte _teamID;
    private final byte _idInMatch;
    private final float _hpRegenAmount, _spRegenAmount;

    private final byte[] _INLCTA;
    private final int _idxLevel = 7;
    private final int _idxClass = 8;
    private boolean _verified;

    private long _lastPacketReceived;
    private long _manaRegenTick;
    private long _manaRegenElapsed;
    private long _hpRegenTick;
    private long _hpRegenElapsed;
    private long _waitForHPRegen;
    private long _hpRegenWaitElapsed;
    private final long _inactivityWarningThreshold = 30000;
    private final long _inactivityMaximumThreshold = 61000;
    private final byte[] _position, _direction;
    private float _currentHP, _currentMana, _maxHP, _maxMana;
    private float _priorHP, _priorMana;
    private float _ley;
    private int _lastPRPacketID;

    public MatchCharacter(PlayerCharacter pc, byte teamID, byte idInMatch, Match match, long hpRegenTick){
        MarkPacketReceived();
        _lastPRPacketID = 0;
        _hpRegenElapsed = 0;
        _hpRegenTick = hpRegenTick;
        _manaRegenElapsed = 0;
        _manaRegenTick = 1000;
        _waitForHPRegen = 10000;
        _position = new byte[12];
        _direction = new byte[12];
        _currentHP = 1;
        _currentMana = 1;
        _verified = false;
        _owningMatch = match;
        _pc = pc;
        _ley = _pc.GetCharacterClass().GetClass() == CharacterClass.Mentalist? 0.6f : 0.0f;

        _maxHP = _pc.GetMaxHP();
        _maxMana = _pc.GetMaxMana();
        _hpRegenAmount = (1 + (_pc.GetMaxHP() / 25));
        _spRegenAmount = (1 + (_pc.GetMaxMana() / 25));
        _pc.SetMatchDetails(idInMatch, match.MatchID(), teamID);
        _teamID = teamID;
        _idInMatch = idInMatch;
        byte[] nameLevelClass = _pc.GetNameLevelClassBytes();
        _INLCTA = new byte[nameLevelClass.length + 7];
        _INLCTA[0] = idInMatch;
        _INLCTA[1] = teamID;
        byte[] appearanceBytes = pc.GetAppearanceBytes();
        System.arraycopy(appearanceBytes, 0, _INLCTA, 2, appearanceBytes.length);
        System.arraycopy(nameLevelClass, 0, _INLCTA, 7, nameLevelClass.length);
    }
    public int GetLastPRPacketID(){
        return _lastPRPacketID;
    }
    public void SetLey(float ley){
        _ley = ley;
    }
    public void Revive(byte reviverID){

    }
    public void TakeDamage(short damageAmount, byte damageSource){
        _hpRegenWaitElapsed = 0;
        _currentHP -= damageAmount;
        if(_currentHP <= 0){
            _owningMatch.PlayerKilled(_idInMatch, damageSource);
            _owningMatch.AdjustPlayerScore(damageSource, 1);
        }
        else{
            _owningMatch.SendToPlayer(Packets.PlayerDamagedPacket(_idInMatch, damageSource, _currentHP), this);
        }
    }

    public float GetRemainingMana(){
        return _currentMana;
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
        return _pc.GetCharacterClass().GetClass();
    }

    public byte[] GetINLCTABytes(){
        return _INLCTA;
    }

    public byte GetTeamID(){
        return _teamID;
    }

    public byte GetIDinMatch(){
        return _idInMatch;
    }

    public String GetCharacterName(){
        return _pc.GetCharacterName();
    }

    public void MarkVerified(){
        Main.LogMessage("Player " + _idInMatch + " verified for team " + _teamID);
        MarkPacketReceived();
        _verified = true;
    }

    public boolean IsVerified(){
        return _verified;
    }

    public RemoteClient GetRemoteClient(){
        return _pc.GetRemoteClient();
    }

    public void MarkPacketReceived(){
        _lastPacketReceived = System.currentTimeMillis();
    }

    public boolean InactivityExceededWarningThreshold(){
        return (System.currentTimeMillis() - _lastPacketReceived) >= _inactivityWarningThreshold;
    }

    public boolean InactivityExceededMaximumThreshold(){
        Main.LogMessage("Inactivity check: " + _lastPacketReceived + ", " + _inactivityMaximumThreshold);
        return (System.currentTimeMillis() - _lastPacketReceived) >= _inactivityMaximumThreshold;
    }
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
            _priorMana = _currentMana;
            return true;
        }
        return false;
    }
    public void AdjustMana(int adjustment){
        _currentMana += adjustment;
    }

    public byte[] GetPosition(){
        return _position;
    }

    protected void UpdatePosition(byte[] decrypted){
        System.arraycopy(decrypted, 7, _position, 0, 12);
    }

    protected void UpdateDirection(byte[] decrypted, int index){
        System.arraycopy(decrypted, index, _direction, 0, 12);
    }

    protected void PlayerDied(MatchCharacter deadPlayer){

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
    @Override
    public String toString(){
        return "MCID: " + _idInMatch + ", TeamID: " + _teamID;
    }
}
