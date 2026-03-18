using UnityEngine;

public class AoEProjectile : Projectile
{
    public float areaOfEffectRadius;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (!_directHit && _impact && (_casterID != MatchParams.IDinMatch))
        {
            if (SharedFunctions.IsPlayerInRadius(transform.position, areaOfEffectRadius))
            {
                Game.SendInGameBytes(InGame_Packets.ReportSplashHit(_castID));                
            }
        }
    }
}
