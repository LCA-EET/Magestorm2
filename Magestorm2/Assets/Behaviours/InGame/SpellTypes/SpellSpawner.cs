using UnityEngine;
public class SpellSpawner : MonoBehaviour 
{
    public byte SpellKey;
    private byte _casterID;
    private short _castID;
    private Team _castingTeam;

    public void Start()
    {
        SpawnedSpell[] childObjects = GetComponentsInChildren<SpawnedSpell>();
        foreach (SpawnedSpell obj in childObjects)
        {
            obj.Initialize(_casterID, _castingTeam, _castID, transform.parent);
        }
        Destroy(gameObject);
    }
    public virtual void InitializeSpell(byte casterID, Team castingTeam, short castID, Vector3 position, Vector3 direction)
    {
        _casterID = casterID;
        _castingTeam = castingTeam;
        _castID = castID;
        Vector3 adjustedPosition = position + (direction * 0.67f);
        adjustedPosition.y += 1.4f;
        transform.position = adjustedPosition;
        transform.forward = direction;
    }
}
