using NUnit.Framework.Internal;
using UnityEngine;
public class SpellSpawner : MonoBehaviour 
{
    public byte SpellKey;
    private byte _casterID;
    protected SpellData _spellReference;
    private byte[] _payload;
    private short _castID;
    private Team _castingTeam;
    private Avatar _caster;

    public void Start()
    {
        SpawnedSpell[] childObjects = GetComponentsInChildren<SpawnedSpell>();
        foreach (SpawnedSpell obj in childObjects)
        {
            obj.Initialize(_casterID, _castingTeam, _castID, transform.parent, _spellReference);
        }
        Destroy(gameObject);
    }
    public void Initialize(Avatar caster, byte spellType, short castID, byte[] payload)
    {
        if (!SpellManager.GetSpell(SpellKey, ref _spellReference))
        {
            Destroy(gameObject);
            return;
        }
        _castID = castID;
        _payload = payload;
        _caster = caster;
        _casterID = _caster.PlayerID;
        _castingTeam = caster.PlayerTeam;
        switch (spellType)
        {
            case ControlCodes.SpellTypes_Self:
                AssociateToCaster();
                break;
            case ControlCodes.SpellTypes_Projectile:
                InitializeProjectile();
                break;
            case ControlCodes.SpellTypes_Summon:
                InitializeSummon();
                break;
            case ControlCodes.SpellTypes_Bolt:
                InitializeBolt();
                break;
        }
        if (_casterID == MatchParams.IDinMatch)
        {
            UseStamina();
        }
    }
    private void InitializeBolt()
    {
        byte targetID = _payload[0];
        Avatar target = null;
        Vector3 direction;        
        if(Match.GetAvatar(targetID, ref target))
        {
            direction = SharedFunctions.DirectionVector(_caster.transform.position, target.transform.position);
        }
        else
        {
            direction = ByteUtils.BytesToVector3(_payload, 1);
            Debug.Log("Direction2: " + direction.ToString());
        }
        SetOrigin(direction);
    }
    private void AssociateToCaster()
    {
        transform.parent = _caster.transform;
        transform.position = _caster.transform.position;
        transform.localPosition = new Vector3(0, 0, 0);
    }

    private void InitializeSummon()
    {
        Debug.Log("Initialize Summon.");
        AssociateToCaster();
        byte summonedPlayerID = _payload[0];
        Avatar summonedPlayer = null;
        if (Match.GetAvatar(summonedPlayerID, ref summonedPlayer))
        {
            if (summonedPlayerID == MatchParams.IDinMatch)
            {
                ComponentRegister.PC.UpdatePosition(_caster.transform.position + ((_caster.transform.forward) * 0.67f));
            }
        }
    }
    private void SetOrigin(Vector3 direction)
    {
        Vector3 adjustedPosition = _caster.transform.position + (direction * 0.67f);
        adjustedPosition.y += 1.4f;
        transform.position = adjustedPosition;
        transform.forward = direction;
    }
    private void InitializeProjectile()
    {
        Vector3 direction = ByteUtils.BytesToVector3(_payload, 0);
        SetOrigin(direction);
    }
    private void UseStamina()
    {
        ComponentRegister.PC.UseStamina(_spellReference.GetStaminaCost(PlayerAccount.SelectedCharacter.CharacterLevel));
    }
}
