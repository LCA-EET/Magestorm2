using UnityEngine;
public class SpawnedSpell : MonoBehaviour 
{
    public AudioClip CastClip;
    protected byte _casterID;
    protected short _castID;
    public float ExpireAfter;
    private float _expiration;
    protected Team _castingTeam;
    public virtual void Initialize(byte casterID, Team castingTeam, short castID, Transform parent)
    {
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
