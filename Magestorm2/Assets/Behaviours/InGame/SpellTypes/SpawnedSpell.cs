using UnityEngine;
public class SpawnedSpell : MonoBehaviour 
{
    public AudioClip CastClip;
    protected byte _casterID;
    protected short _castID;
    public float ExpireAfter;
    public float DestroyAfter;
    private float _expiration;
    private float _destructionElapsed;
    protected SpellData _spellReference;
    protected Team _castingTeam;
    protected bool _destroyOnNextUpdate;
    public virtual void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        _spellReference = spellReference;
        _expiration = Time.realtimeSinceStartup + (ExpireAfter == 0 ? 60 : ExpireAfter);
        transform.parent = parent;
        _casterID = casterID;
        _castingTeam = castingTeam;
        _castID = castID;
        ComponentRegister.Spawner.RegisterSpawnedSpell(this);
        if(casterID == MatchParams.IDinMatch && CastClip != null)
        {
            if(CastClip!= null)
            {
                ComponentRegister.AudioPlayer.PlayClip(CastClip);
            }
        }
    }
    public bool IsExpired(float currentTime)
    {
        return currentTime >= _expiration;
    }
    public virtual void Update()
    {
        if (_destroyOnNextUpdate)
        {
            if(_destructionElapsed >= DestroyAfter)
            {
                ComponentRegister.Spawner.DeregisterSpawnedSpell(this);
                Destroy(gameObject);
            }
            else
            {
                _destructionElapsed += Time.deltaTime;
            }
        }
    }
    public void MarkForDestruction()
    {
        _destroyOnNextUpdate = true;
    }
    public short CastID
    {
        get { return _castID; }
    }
    public byte CasterID
    {
        get { return _casterID; }
    }
}
