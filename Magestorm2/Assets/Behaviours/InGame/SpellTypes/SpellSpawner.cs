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
    private Vector3 _boltYAdjustment;
    public void Start()
    {
        _boltYAdjustment = new Vector3(0, 0.5f, 0);
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
            case ControlCodes.SpellTypes_PBAoE:
                AssociateToCaster(1.0f);
                break;
            case ControlCodes.SpellTypes_Self:
                AssociateToCaster(0.0f);
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
            case ControlCodes.SpellTypes_NonSolidWall:
            case ControlCodes.SpellTypes_SolidWall:
                InitializeWall();
                break;
        }
        if (_casterID == MatchParams.IDinMatch)
        {
            UseStamina();
        }
    }
    private void InitializeWall()
    {
        Vector3 position = ByteUtils.BytesToVector3(_payload, 0);
        Vector3 eulers = ByteUtils.BytesToVector3(_payload, 12);
        transform.eulerAngles = eulers;
        transform.position = position;
    }
    private void InitializeBolt()
    {
        byte targetID = _payload[0];
        Debug.Log("Bolt target: " + targetID);
        Avatar target = null;
        Vector3 direction;        
        if(Match.GetAvatar(targetID, ref target))
        {
            direction = SharedFunctions.DirectionVector(_caster.transform.position, target.transform.position - _boltYAdjustment);
            Debug.Log("Direction2: " + direction.ToString());
        }
        else
        {
            direction = ByteUtils.BytesToVector3(_payload, 1);
            Debug.Log("Direction3: " + direction.ToString());
        }
        //direction.y -= 0.5f;
        SetOrigin(direction);
    }
    private void AssociateToCaster(float y)
    {
        transform.parent = _caster.transform;
        transform.position = _caster.transform.position;
        transform.localPosition = new Vector3(0, y, 0);
    }

    private void InitializeSummon()
    {
        Debug.Log("Initialize Summon.");
        AssociateToCaster(1.0f);
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
