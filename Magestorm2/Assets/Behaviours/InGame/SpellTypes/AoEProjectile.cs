using UnityEngine;

public class AoEProjectile : Projectile
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (!_directHit && _impact && (_casterID != MatchParams.IDinMatch))
        {
            if (SharedFunctions.IsPlayerInRadius(transform.position, _spellReference.EffectRadius))
            {
                Debug.Log("Splash hit!");
                Game.SendInGameBytes(InGame_Packets.ReportSplashHit(_castID));                
            }
        }
    }
}
