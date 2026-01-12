using UnityEngine;
public class SpawnedSpell : MonoBehaviour 
{
    protected byte _casterID;
    protected short _castID;
    protected Team _castingTeam;
    protected PeriodicAction _destroyOnExpiration;
    public void Initialize(byte casterID, Team castingTeam, short castID, Transform parent)
    {
        _destroyOnExpiration = new PeriodicAction(60.0f, DestroySelf, null);
        transform.parent = parent;
        _casterID = casterID;
        _castingTeam = castingTeam;
        _castID = castID;
    }
    public virtual void Update()
    {
        _destroyOnExpiration.ProcessAction(Time.deltaTime);
    }
    private void DestroySelf()
    {
        Debug.Log("Destroying expired spell: " + _castID);
        Destroy(gameObject);
    }
}
