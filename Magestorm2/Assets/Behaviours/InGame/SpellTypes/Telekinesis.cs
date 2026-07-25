using UnityEngine;

public class Telekinesis : Bolt
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (_casterID == Game.PCAvatar.PlayerID)
        {
            ActivateableObject hitObject = other.GetComponent<ActivateableObject>();
            if (hitObject != null)
            {
                SpawnImpactPrefab();
                hitObject.StateChangeRequest();
                return;
            }
        }
        base.OnTriggerEnter(other);
    }
    protected override void OnWallHit(Wall hitWall)
    {
        return;
    }
    protected override void OnRemoteHit()
    {
        return;
    }
}