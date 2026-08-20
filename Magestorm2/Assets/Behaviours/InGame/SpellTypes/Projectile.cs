using UnityEngine;
public class Projectile : OutwardCast
{
    public Woosh Woosh = Woosh.None;
    private bool _wooshPlayed;
    protected RaycastHit _hitInfo;
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

            if(SharedFunctions.AdvanceCast(transform, toAdvance, _impactMask, out _hitInfo))
            {
                OnTriggerEnter(_hitInfo.collider);
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
                Game.Clips.PlayWoosh(Woosh, Game.PCAvatar.AudioSource);
            } 
        }
        else
        {
            if(other.gameObject.layer == LayerManager.BiasableLayer)
            {
                //Debug.Log("Biasable object hit.");
                BiasableTrigger bt = other.gameObject.GetComponent<BiasableTrigger>();
                if(bt != null)
                {
                    if (bt.TriggerType == TriggerType.ManaPool)
                    {
                        ComponentRegister.Spawner.SpawnVFX(ControlCodes.VFX_Splash, transform.position);
                        //Debug.Log("Transform position: " + transform.position);
                        MarkForDestruction();
                    }
                }
            }
            else
            {
                base.OnTriggerEnter(other);
            }
        }
    }
}
