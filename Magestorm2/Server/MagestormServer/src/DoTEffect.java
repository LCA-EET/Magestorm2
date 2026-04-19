public class DoTEffect extends AppliedEffect{
    private long _elapsedSinceLastHit;
    private final float _damagePerTick;
    public DoTEffect(MatchCharacter caster, MatchCharacter target, Spell baseReference, byte degree, byte effectCode,
                     byte duration, float damagePerTick) {
        super(caster, target, baseReference, degree, effectCode, duration);
        _damagePerTick = damagePerTick;
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
