public class DoTEffect extends AppliedEffect{
    private long _elapsedSinceLastHit;
    private final float _damagePerTick;
    public DoTEffect(MatchCharacter caster, MatchCharacter target, Effect baseReference, byte degree, short effectID,
                     float spellDamage) {
        super(caster, target, baseReference, degree, effectID);
        _damagePerTick = (spellDamage * baseReference.PercentOverTime()) / baseReference.GetDuration();
    }

    @Override
    public boolean ReduceDuration(long msElapsed){
        boolean expired = super.ReduceDuration(msElapsed);
        if(!expired){
            _elapsedSinceLastHit += msElapsed;
            if(_elapsedSinceLastHit >= 1000){
                _elapsedSinceLastHit = 0;
                if(_target.IsAlive()){
                    _target.TakeDamage(_damagePerTick, _caster);
                }
            }
        }
        return expired;
    }
}
