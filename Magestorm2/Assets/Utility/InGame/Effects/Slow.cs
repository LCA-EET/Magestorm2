using UnityEngine;

public class Slow : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkSlow(true);
        }
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        ComponentRegister.PlayerMovement.MarkSlow(false);
    }
}
