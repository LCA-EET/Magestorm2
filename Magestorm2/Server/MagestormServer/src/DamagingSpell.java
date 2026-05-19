public class DamagingSpell extends CastSpell{
    protected short _damage0, _damage1;
    protected boolean _multiElement;
    public DamagingSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference){
        super(caster, castID, baseReference, matchReference);
        _multiElement = (baseReference.GetMinDamagePerRoll1() > 0);
    }

    private void CalculateDamage0()
    {
        byte minRoll0 = _baseReference.GetMinDamagePerRoll0();
        byte maxRoll0 = _baseReference.GetMaxDamagePerRoll0();
        _damage0 = GameUtils.DiceRoll(minRoll0, maxRoll0, _baseReference.GetNumRolls());
    }

    private void CalculateDamage1(){
        byte minRoll1 = _baseReference.GetMinDamagePerRoll1();
        byte maxRoll1 = _baseReference.GetMaxDamagePerRoll1();
        _damage1 = GameUtils.DiceRoll(minRoll1, maxRoll1, _baseReference.GetNumRolls());
    }

    public short GetDamage0(){
        if(_damage0 == 0){
            CalculateDamage0();
        }
        return _damage0;
    }

    public short GetDamage1(){
        if(_damage1 == 0){
            CalculateDamage1();
        }
        return _damage1;
    }

    protected void ApplyDamage(short damage, byte element, MatchCharacter target){
        float appliedDamage = damage;
        float resistance = target.GetResistance(element);
        if(resistance != 0){
            Main.LogDebug("DamagingSpell.ApplyDamage(): Pre-resist appliedDamage: " + appliedDamage);
            appliedDamage *= (1.0f - resistance );
            Main.LogDebug("DamagingSpell.ApplyDamage(): Post-resist appliedDamage: " + appliedDamage);
        }
        if(target.IsSplashHit((int)_objectID)){
            byte skillLevel = _casterReference.GetSkillLevel(_baseReference.GetDisciplineCode());
            appliedDamage *= Math.max(1,_baseReference.GetSplashFactor(skillLevel));
            Main.LogMessage("DamagingSpell.ApplyDamage(): Splash hit applied damage = " + appliedDamage);
        }
        else{
            Main.LogMessage("DamagingSpell.ApplyDamage(): Not a splash hit.");
        }
        if(target.IsShocked()){
            appliedDamage *= 1.1f;
        }
        target.TakeDamage(appliedDamage, _casterReference);
        if(target.GetIDinMatch() != _casterReference.GetIDinMatch()){
            _casterReference.AdjustExperience(appliedDamage * 2);
        }
    }

    @Override
    public void ProcessSpell(MatchCharacter affectedPlayer){
        Main.LogDebug("DamagingSpell.ProcessSpell(): PriorHP: " + affectedPlayer.GetCurrentHP());
        ApplyDamage(GetDamage0(), _baseReference.GetElement0(), affectedPlayer);
        if(_multiElement){
            ApplyDamage(GetDamage1(), _baseReference.GetElement1(), affectedPlayer);
        }
        Main.LogDebug("DamagingSpell.ProcessSpell(): CurrentHP: " + affectedPlayer.GetCurrentHP());
        if(affectedPlayer.IsAlive()){
            ProcessEffect(affectedPlayer);
        }
    }

    @Override
    protected AppliedEffect CreateEffect(MatchCharacter target, Effect baseEffect){
        return new DoTEffect(_casterReference, target, baseEffect, _spellLevel, GetDamage0());
    }
}
