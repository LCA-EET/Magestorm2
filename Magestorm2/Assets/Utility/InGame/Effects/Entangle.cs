using UnityEngine;
public class Entangle : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkEntangled(true);
        }
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        ComponentRegister.PlayerMovement.MarkEntangled(false);
    }
}
