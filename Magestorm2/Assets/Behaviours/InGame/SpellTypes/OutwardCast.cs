using UnityEngine;
public class OutwardCast : SpawnedSpell
{
    public SpellImpact ImpactPrefab;
    public bool InvertImpactDirection;
    public float ImpactScaling = 1.0f;
    public bool RecursiveImpactScaling = false;
    public byte ShieldVFX;
    
    protected int _impactMask;
    protected bool _impact, _directHit;
    protected Avatar _hitPlayer;
    protected virtual void SpawnImpactPrefab()
    {
        Debug.Log("Spawning Impact Prefab - Scale: " + ImpactScaling);
        SpellImpact impactObject = Instantiate(ImpactPrefab);
        if (ImpactScaling > 0)
        {
            Vector3 newScale = Vector3.one * ImpactScaling;
            ApplyRecursiveScaling(impactObject.transform, newScale);
        }
        impactObject.transform.position = transform.position;
        if (InvertImpactDirection)
        {
            impactObject.InvertDirection(transform.forward);
        }
    }
    private void ApplyRecursiveScaling(Transform goTransform, Vector3 newScale)
    {
        goTransform.localScale = newScale;
        if (RecursiveImpactScaling)
        {
            for (int i = 0; i < goTransform.childCount; i++)
            {
                ApplyRecursiveScaling(goTransform.GetChild(i), newScale);
            }
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
                if (SharedFunctions.WasRemoteHit(other, out _hitPlayer))
                {
                    OnRemoteHit();
                }
                Wall hitWall = null;
                if (_casterID == MatchParams.IDinMatch)
                {
                    if (SharedFunctions.WasWallHit(other, out hitWall))
                    {
                        OnWallHit(hitWall);
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
    protected virtual void OnWallHit(Wall hitWall)
    {
        Game.SendInGameBytes(InGame_Packets.WallHitPacket(_castID, hitWall.CastID));
    }
    protected virtual void OnRemoteHit()
    {
        byte shieldID = SharedFunctions.IsShieldedFromElement(_spellReference.Element0, _hitPlayer);
        if (shieldID > 0 && ShieldVFX != ControlCodes.VFX_None)
        {
            ComponentRegister.Spawner.SpawnVFX(ShieldVFX, _hitPlayer.transform);
        }
    }
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
        _impactMask = LayerManager.ProjectileImpactMask;
    }
}
