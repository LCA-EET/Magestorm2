using UnityEngine;

public class Bolt : Projectile
{
    private float _distanceTravelled;
    private byte _maxRange;
    private Vector3 _priorPosition;
    private ParticleSystem[] _particleSystems;
    private ParticleSystem.EmissionModule _emitter;
    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!_destroyOnNextUpdate)
        {
            _distanceTravelled += Vector3.Distance(_priorPosition, transform.position);
            //Debug.Log("DT: " + _distanceTravelled);
            _priorPosition = transform.position;
            if (_distanceTravelled >= _maxRange)
            {
                MarkForDestruction();
            }
        }
        else
        {
            // Stop emitting new particles. The gameObject is destroyed in SpawnedSpell.Update()

            foreach (ParticleSystem particle in _particleSystems)
            {
                ParticleSystem.EmissionModule emitter = particle.emission;
                emitter.enabled = false;
            }
        }
    }
    
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        _maxRange = spellReference.Range;
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
        _priorPosition = transform.position;
    }
}
