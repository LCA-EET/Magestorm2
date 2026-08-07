using UnityEngine;
public class Projectile : OutwardCast
{
    public Woosh Woosh = Woosh.None;
    private bool _wooshPlayed;
    protected virtual void FixedUpdate()
    {
        if (!_destroyOnNextUpdate)
        {
            Propel(Time.fixedDeltaTime);
        }
    }
    protected void Propel(float delta)
    {
        if (!_destroyOnNextUpdate)
        {
            float toAdvance = _spellReference.ProjectileSpeed * delta;
            transform.position += (transform.forward * toAdvance);
            
            RaycastHit hitInfo;
            if(SharedFunctions.AdvanceCast(transform, toAdvance, _impactMask, out hitInfo))
            {
                OnTriggerEnter(hitInfo.collider);
            }
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerManager.WooshLayer)
        {
            if(Woosh != Woosh.None && !_wooshPlayed && CasterID != MatchParams.IDinMatch)
            {
                _wooshPlayed = true;
                ComponentRegister.AudioPlayer.PlayWoosh(Woosh);
            } 
        }
        else
        {
            base.OnTriggerEnter(other);
        }
    }
}
