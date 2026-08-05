using UnityEngine;
public class Projectile : OutwardCast
{
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

}
