using UnityEngine;

public class Haste : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkHaste(true);
        }
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkHaste(false);
        }
    }
}
