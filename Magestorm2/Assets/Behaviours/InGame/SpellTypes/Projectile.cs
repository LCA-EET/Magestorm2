using UnityEngine;

namespace Magus
{
    public class Projectile : OutwardCast
    {
        public Woosh Woosh = Woosh.None;
        private bool _wooshPlayed;
        protected RaycastHit _hitInfo;
        protected Vector3 _deltaY;
        protected Vector3 _downwardCastPositionAdjustment, _downwardCast;
        protected byte _speed;
        protected bool _rolling, _gravity;
        protected override void Awake()
        {
            base.Awake();
            if(_rolling)
            {
                _downwardCastPositionAdjustment = new Vector3(0, (transform.localScale.y / 2), 0);
                _downwardCast = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
            }
        }

        protected virtual void Start()
        {
            _speed = _spellReference.ProjectileSpeed;
            _rolling = _spellReference.Rolling;
            _gravity = _spellReference.Gravity;
        }
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
                Vector3 adjustment = Vector3.zero;
                if (_gravity)
                {
                    if (_rolling)
                    {
                        AdjustForGravity(ref adjustment, delta);
                    }
                    else
                    {
                        if (IsOnGround())
                        {
                            LevelOffRollingProjectile();
                        }
                        else
                        {
                            AdjustForGravity(ref adjustment, delta);
                        }
                    }
                }
                else
                {
                    if (_rolling)
                    {
                        if (IsOnGround())
                        {
                            LevelOffRollingProjectile();
                        }
                    }
                }
                float advancement = AdjustForward(ref adjustment, delta);
                transform.position += adjustment;
                if (SharedFunctions.AdvanceCast(transform, advancement, _impactMask, out _hitInfo))
                {
                    OnTriggerEnter(_hitInfo.collider);
                }
            }
        }
        protected void LevelOffRollingProjectile()
        {
            _deltaY = Vector3.zero;
            transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
        }
        protected float AdjustForward(ref Vector3 adjustment, float delta)
        {
            float toAdvance = _speed * delta;
            adjustment += transform.forward * toAdvance;
            return toAdvance;
        }
        protected void AdjustForGravity(ref Vector3 adjustment, float delta)
        {
            _deltaY += delta * SharedFunctions.Gravity;
            adjustment -= _deltaY * delta;
        }
        protected bool IsOnGround()
        {
            return Physics.OverlapBox(transform.position - _downwardCastPositionAdjustment, _downwardCast,
                        Quaternion.identity, LayerManager.FloorMask).Length > 0;
        }
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerManager.WooshLayer)
            {
                if (Woosh != Woosh.None && !_wooshPlayed && CasterID != MatchParams.IDinMatch)
                {
                    _wooshPlayed = true;
                    Game.Clips.PlayWoosh(Woosh, Game.PCAvatar.AudioSource);
                }
            }
            else
            {
                if (other.gameObject.layer == LayerManager.BiasableLayer)
                {
                    //Debug.Log("Biasable object hit.");
                    BiasableTrigger bt = other.gameObject.GetComponent<BiasableTrigger>();
                    if (bt != null)
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
}

