using UnityEngine;

public class Telekinesis : Bolt
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (!_destroyOnNextUpdate)
        {
            if (_casterID == Game.PCAvatar.PlayerID)
            {
                ActivateableObject hitObject = other.GetComponent<ActivateableObject>();
                if (hitObject != null)
                {
                    hitObject.StateChangeRequest();
                }
            }
            base.OnTriggerEnter(other);
        }
        
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