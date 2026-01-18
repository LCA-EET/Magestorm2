using UnityEngine;
public class Projectile : SpawnedSpell
{
    public float Speed;
    public GameObject ImpactPrefab;
    
    private void FixedUpdate()
    {
        transform.position += (Speed * transform.forward * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ImpactPrefab != null)
        {
            GameObject impactObject = Instantiate(ImpactPrefab);
            impactObject.transform.position = transform.position;
        }
        if (SharedFunctions.WasPlayerHit(other))
        {
            if (ComponentRegister.PC.IsAlive)
            {
                Game.SendInGameBytes(InGame_Packets.ReportHitPacket(_castID));
            }
        }
        Destroy(gameObject);
    }
}
