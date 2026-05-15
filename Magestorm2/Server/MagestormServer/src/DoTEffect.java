public class DoTEffect extends AppliedEffect{
    private long _elapsedSinceLastHit;
    private float _damagePerTick;
    public DoTEffect(MatchCharacter caster, MatchCharacter target, Effect baseReference, byte degree,
                     float spellDamage) {
        super(caster, target, baseReference, degree);
        _damagePerTick = (spellDamage * baseReference.PercentOverTime()) / baseReference.GetDuration();
        //_damagePerTick += (degree - 1) * (0.5f * _damagePerTick);
    }

    @Override
    public boolean Tick(long msElapsed){
        _elapsedSinceLastHit += msElapsed;
        if(_elapsedSinceLastHit >= 1000){
            _elapsedSinceLastHit = 0;
            if(_target.IsAlive()){
                _target.TakeDamage(_damagePerTick, _caster);
            }
        }
        return super.Tick(msElapsed);
    }
}
