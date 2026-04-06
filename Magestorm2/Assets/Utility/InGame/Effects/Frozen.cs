using UnityEngine;

public class Frozen : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkFrozen(true);
        }
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        ComponentRegister.PlayerMovement.MarkFrozen(false);
    }
}
