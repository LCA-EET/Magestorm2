using UnityEngine;
public class Projectile : SpawnedSpell
{
    public float Speed;
    public GameObject ImpactPrefab;
    public float ImpactScaling = 1.0f;
    private void FixedUpdate()
    {
        
        transform.position += (Speed * transform.forward * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        bool impact = true;
        // The target will report on being hit.
        // The shooter does NOT report on what it hit.
        // Shooter's projectile cannot impact themself
        
        if (SharedFunctions.WasPCHit(other))
        {
            //Debug.Log("WPH is true. CasterID: " + CasterID);
            if (CasterID == MatchParams.IDinMatch)
            {
                impact = false;
            }
            else
            {
                Game.SendInGameBytes(InGame_Packets.ReportHitPacket(_castID));
            }
        }

        if (ImpactPrefab != null && impact)
        {
            GameObject impactObject = Instantiate(ImpactPrefab);
            impactObject.transform.localScale = Vector3.one * ImpactScaling;
            impactObject.transform.position = transform.position;
        }
        if (impact)
        {
            Destroy(gameObject);
        }
    }
}
