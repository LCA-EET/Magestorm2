using UnityEngine;
public class FriendlyProjectile : Projectile 
{
    public VFXCode VFXCode;
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
