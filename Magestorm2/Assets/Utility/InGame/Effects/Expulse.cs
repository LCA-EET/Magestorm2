public class Expulse : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if(appliedTo.PlayerID == Game.PCAvatar.PlayerID)
        {
            ComponentRegister.PlayerMovement.ApplyForceVector(5 + (_degree * 5), 1.5f, transform.up);
        }
    }
}
