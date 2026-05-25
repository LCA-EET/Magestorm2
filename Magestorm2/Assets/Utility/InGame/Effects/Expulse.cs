public class Expulse : AppliedEffect
{
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if(appliedTo.PlayerID == Game.PCAvatar.PlayerID)
        {
            ComponentRegister.PlayerMovement.ApplyExpulse(_degree);
        }
    }
}
