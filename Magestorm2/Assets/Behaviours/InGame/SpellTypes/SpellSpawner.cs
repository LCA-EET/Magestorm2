using UnityEngine;
using UnityEngine.UI;
public class SpellSpawner : MonoBehaviour 
{
    public byte SpellKey;
    private byte _casterID;
    private byte[] _payload;
    private short _castID;
    private Team _castingTeam;
    private Avatar _caster;

    public void Start()
    {
        SpawnedSpell[] childObjects = GetComponentsInChildren<SpawnedSpell>();
        foreach (SpawnedSpell obj in childObjects)
        {
            obj.Initialize(_casterID, _castingTeam, _castID, transform.parent);
        }
        Destroy(gameObject);
    }
    public void Initialize(Avatar caster, byte spellType, short castID, byte[] payload)
    {
        _castID = castID;
        _payload = payload;
        _caster = caster;
        switch (spellType)
        {
            case ControlCodes.SpellTypes_Self:
                InitializeSelfCast();
                break;
            case ControlCodes.SpellTypes_Projectile:
                InitializeProjectile();
                break;
            case ControlCodes.SpellTypes_Summon:
                break;
        }
        if (_caster.PlayerID == MatchParams.IDinMatch)
        {
            UseStamina();
        }
    }

    private void InitializeSelfCast()
    {
        transform.parent = _caster.transform;
        transform.position = _caster.transform.position;
        transform.localPosition = new Vector3(0, 0, 0);
    }
    private void InitializeProjectile()
    {
        Vector3 direction = ByteUtils.BytesToVector3(_payload, 0);
        Vector3 adjustedPosition = _caster.transform.position + (direction * 0.67f);
        Debug.Log("Direction: " + direction);
        adjustedPosition.y += 1.4f;
        transform.position = adjustedPosition;
        transform.forward = direction;
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
