using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AppliedEffect
{
    private EffectCode _effectCode;
    private Avatar _appliedTo, _applier;
    private float _duration;
    public AppliedEffect(EffectCode effectCode, Avatar applier,  float duration)
    {
        _applier = applier;
        _effectCode = effectCode;
        _duration = duration;
    }
    public bool Tick(float deltaTime)
    {
        _duration -= deltaTime;
        return _duration <= 0;
    }
    public virtual void ApplyEffect(Avatar appliedTo)
    {
        _appliedTo = appliedTo;
    }
    public virtual void ReverseEffect()
    {

    }
    public EffectCode EffectCode
    {
        get { return _effectCode; }
    }
    public float TimeRemaining
    {
        get { return _duration; }
    }
}
