using UnityEngine;
using UnityEngine.UI;
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
    public virtual void InitializeSelfCast(Avatar caster, short castID)
    {
        Debug.Log("InitializedSelfCast");
        _castID = castID;
        transform.parent = caster.transform;
        transform.position = caster.transform.position;
        transform.localPosition = new Vector3(0, 0, 0);
        if(caster.PlayerID == MatchParams.IDinMatch)
        {
            UseStamina();
        }
    }
    public virtual void InitializeProjectile(byte casterID, Team castingTeam, short castID, Vector3 position, Vector3 direction)
    {
        _casterID = casterID;
        _castingTeam = castingTeam;
        _castID = castID;
        Vector3 adjustedPosition = position + (direction * 0.67f);
        adjustedPosition.y += 1.4f;
        transform.position = adjustedPosition;
        transform.forward = direction;
        if(_casterID == MatchParams.IDinMatch)
        {
            UseStamina();
        }
    }
    private void UseStamina()
    {
        SpellData data = null;
        if (SpellManager.GetSpell(SpellKey, ref data))
        {
            ComponentRegister.PC.UseStamina(data.GetStaminaCost(PlayerAccount.SelectedCharacter.CharacterLevel));
        }
    }
}
