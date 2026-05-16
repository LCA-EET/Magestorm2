using UnityEngine;
public class Fly : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkInFlight(true);
        }
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        if (_appliedToPlayer)
        {
            ComponentRegister.PlayerMovement.MarkInFlight(false);
        }
    }
}
