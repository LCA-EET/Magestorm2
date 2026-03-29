using UnityEngine;
public class Projectile : SpawnedSpell
{
    public GameObject ImpactPrefab;
    public float ImpactScaling = 1.0f;
    protected bool _impact, _directHit;
    protected virtual void FixedUpdate()
    {
        if (!_destroyOnNextUpdate)
        {
            Propel();
        }
    }
    protected void Propel()
    {
        float toAdvance = _spellReference.ProjectileSpeed * Time.deltaTime;
        transform.position += (transform.forward * toAdvance);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, toAdvance, LayerManager.ProjectileImpactMask))
        {
            transform.position = hitInfo.point;
            OnTriggerEnter(hitInfo.collider);
            Debug.Log("Advance hit: " + hitInfo.transform.gameObject.name);
        }
    }
    protected void SpawnImpactPrefab()
    {
        GameObject impactObject = Instantiate(ImpactPrefab);
        impactObject.transform.localScale = Vector3.one * ImpactScaling;
        impactObject.transform.position = transform.position;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        _impact = true;
        Debug.Log("Projectile.OnTriggerEnter");
        // The target will report on being hit.
        // The shooter does NOT report on what it hit.
        // Shooter's projectile cannot impact themself

        if (SharedFunctions.WasPCHit(other) && Game.PCAvatar.IsAlive)
        {
            Debug.Log("WPH is true. CasterID: " + CasterID);
            if (CasterID == MatchParams.IDinMatch)
            {
                _impact = false;
            }
            else
            {
                _directHit = true;
                Debug.Log("Direct hit!");
                Game.SendInGameBytes(InGame_Packets.ReportHitPacket(_castID));
                SharedFunctions.CameraShake(_spellReference);
            }
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
