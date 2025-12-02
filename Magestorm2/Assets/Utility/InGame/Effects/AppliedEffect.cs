using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EffectCode : byte
{
    Haste = 0,
    Slow = 1,
    Freezing = 2,
    Burning = 3,
    Shocked = 4,
    Grounded = 5,
    FireShield = 6,
    ColdShield = 7,
    ElectricShield = 8,
    EarthShield = 9,
    Bleeding = 10
}

public class AppliedEffect
{
    private EffectCode _effectCode;
    public AppliedEffect(EffectCode effectCode)
    {
        _effectCode = effectCode;
    }
    public virtual void ApplyEffect()
    {

    }
    public virtual void ReverseEffect()
    {

    }
    public EffectCode EffectCode
    {
        get { return _effectCode; }
    }
}
