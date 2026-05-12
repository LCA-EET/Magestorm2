using UnityEngine;

public class Bolt : Projectile
{
    private float _distanceTravelled;
    private byte _maxRange;
    private Vector3 _priorPosition;
    public ParticleSystem ParticleSystem;
    private ParticleSystem.EmissionModule _emitter;
    private void Awake()
    {
        if (ParticleSystem != null)
        {
            _emitter = ParticleSystem.emission;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!_destroyOnNextUpdate)
        {
            _distanceTravelled += Vector3.Distance(_priorPosition, transform.position);
            _priorPosition = transform.position;
            if (_distanceTravelled >= _maxRange)
            {
                MarkForDestruction();
            }
        }
        else
        {
            if(ParticleSystem != null)
            {
                if (_emitter.enabled)
                {
                    _emitter.enabled = false;
                }
            }
        }
    }
    
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        _maxRange = spellReference.Range;
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        _priorPosition = transform.position;
    }
}
