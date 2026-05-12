using Unity.Collections;
using UnityEngine;
public class Projectile : SpawnedSpell
{
    public SpellImpact ImpactPrefab;
    public bool InvertImpactDirection;
    public float ImpactScaling = 1.0f;
    public byte ShieldVFX;
    protected bool _impact, _directHit;
    protected Avatar _hitPlayer;
    protected int _impactMask;
    protected virtual void FixedUpdate()
    {
        if (!_destroyOnNextUpdate)
        {
            Propel();
        }
    }
    protected void Propel()
    {
        if (!_destroyOnNextUpdate)
        {
            float toAdvance = _spellReference.ProjectileSpeed * Time.deltaTime;
            transform.position += (transform.forward * toAdvance);
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, toAdvance, _impactMask))
            {
                transform.position = hitInfo.point;
                OnTriggerEnter(hitInfo.collider);
                Debug.Log("Advance hit: " + hitInfo.transform.gameObject.name);
            }
        }
    }
    protected virtual void SpawnImpactPrefab()
    {
        Debug.Log("Spawning Impact Prefab");
        SpellImpact impactObject = Instantiate(ImpactPrefab);
        if(ImpactScaling > 0)
        {
            impactObject.transform.localScale = Vector3.one * ImpactScaling;
        }
        impactObject.transform.position = transform.position;
        if (InvertImpactDirection)
        {
            impactObject.InvertDirection(transform.forward);
        }
    }
    protected virtual void ReportHit()
    {
        Game.SendInGameBytes(InGame_Packets.ReportHitPacket(_castID));
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!_destroyOnNextUpdate)
        {
            Debug.Log("Projectile.OnTriggerEnter. Collided with: " + other.name);
            if (SharedFunctions.WasPCHit(other))
            {
                Debug.Log("WPH is true. CasterID: " + CasterID);
                if (CasterID == MatchParams.IDinMatch)
                {
                    _impact = false;
                }
                else
                {
                    _impact = true;
                    _directHit = true;
                    _hitPlayer = Game.PCAvatar;
                    Debug.Log("Direct hit!");
                    ReportHit();
                    SharedFunctions.CameraShake(_spellReference);
                }
            }
            else
            {
                _hitPlayer = null;
                if(SharedFunctions.WasRemoteHit(other, out _hitPlayer))
                {
                    byte shieldID = SharedFunctions.IsShieldedFromElement(_spellReference.Element0, _hitPlayer);
                    if(shieldID > 0 && ShieldVFX != ControlCodes.VFX_None)
                    {
                        ComponentRegister.Spawner.SpawnVFX(ShieldVFX, _hitPlayer.transform);
                    }
                }
                Wall hitWall = null;
                if (_casterID == MatchParams.IDinMatch)
                {
                    if (SharedFunctions.WasWallHit(other, out hitWall))
                    {
                        Game.SendInGameBytes(InGame_Packets.WallHitPacket(_castID, hitWall.CastID));
                        Debug.Log("Wall hit.");
                    }
                }
                _impact = true;
            }
            if (ImpactPrefab != null && _impact)
            {
                SpawnImpactPrefab();
            }
            if (_impact)
            {
                MarkForDestruction();
            }
        }
    }
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        _impactMask = LayerManager.ProjectileImpactMask;
    }
}
