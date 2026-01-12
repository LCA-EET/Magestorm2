using UnityEngine;
public class Projectile : SpawnedSpell
{
    public float Speed;
    public GameObject ImpactPrefab;
    
    private void FixedUpdate()
    {
        transform.position += (Speed * transform.forward * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
    }
    private void OnDestroy()
    {
        if (ImpactPrefab != null)
        {
            GameObject impactObject = Instantiate(ImpactPrefab);
            impactObject.transform.position = transform.position;
        }
    }
}
