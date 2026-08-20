using UnityEngine;
public class SpawnedSpell : MonoBehaviour 
{
    public AudioClip CastClip;
    protected byte _casterID;
    protected short _castID;
    protected Avatar _casterReference;
    public float ExpireAfter;
    public float DestroyAfter;
    private float _expiration;
    private float _destructionElapsed;
    protected SpellData _spellReference;
    protected Team _castingTeam;
    protected bool _destroyOnNextUpdate;
    protected byte[] _payload;
    public virtual void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        if(Match.GetAvatar(casterID, ref _casterReference))
        {
            _casterID = casterID;
            InitializeNoCaster(castingTeam, castID, parent, spellReference, payload);
            if (casterID == MatchParams.IDinMatch && CastClip != null)
            {
                if (CastClip != null)
                {
                    Game.Clips.PlayClip(CastClip, Game.PCAvatar.AudioSource);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public virtual void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        _payload = payload;
        _spellReference = spellReference;
        _expiration = Time.realtimeSinceStartup + (ExpireAfter == 0 ? 60 : ExpireAfter);
        transform.parent = parent;
        _castingTeam = castingTeam;
        _castID = castID;
        if (_spellReference.ForceDuration > 0)
        {
            Match.AddStoredVector(_castID, transform.forward);
        }
        ComponentRegister.Spawner.RegisterSpawnedSpell(this);
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
        Debug.Log("MarkForDestruction");
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
