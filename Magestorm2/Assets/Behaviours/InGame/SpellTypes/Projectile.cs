using UnityEngine;
public class Projectile : SpawnedSpell
{
    public float Speed;
    public GameObject ImpactPrefab;
    public float ImpactScaling = 1.0f;
    protected bool _impact, _directHit;
    private void FixedUpdate()
    {
        transform.position += (Speed * transform.forward * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        _impact = true;
        // The target will report on being hit.
        // The shooter does NOT report on what it hit.
        // Shooter's projectile cannot impact themself
        
        if (SharedFunctions.WasPCHit(other))
        {
            //Debug.Log("WPH is true. CasterID: " + CasterID);
            if (CasterID == MatchParams.IDinMatch)
            {
                _impact = false;
            }
            else
            {
                _directHit = true;
                Game.SendInGameBytes(InGame_Packets.ReportHitPacket(_castID));
            }
        }
        if (ImpactPrefab != null && _impact)
        {
            GameObject impactObject = Instantiate(ImpactPrefab);
            impactObject.transform.localScale = Vector3.one * ImpactScaling;
            impactObject.transform.position = transform.position;
        }
        if (_impact)
        {
            MarkForDestruction();
        }
    }
}
