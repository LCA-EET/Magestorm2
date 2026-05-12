using UnityEngine;
public class FriendlyProjectile : Projectile 
{
    public byte VFXCode;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(_impact && _hitPlayer != null)
        {
            if(_casterReference.PlayerTeam == _hitPlayer.PlayerTeam)
            {
                ComponentRegister.Spawner.SpawnVFX(VFXCode, _hitPlayer.transform);
            }
        }
    }
}
