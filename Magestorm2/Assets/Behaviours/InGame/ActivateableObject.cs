using UnityEngine;

public class ActivateableObject : MonoBehaviour
{
    public byte ObjectKey;
    protected byte _objectStatus;

    private void Awake()
    {
        _objectStatus = 0; // default
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void RegisterObject()
    {

    }

    public void PlayerChangedStatus()
    {
        Match.Send(InGame_Packets.ChangedObjectStatePacket(ObjectKey, _objectStatus));
    }
    public void StatusChanged(byte newStatus)
    {
        _objectStatus = newStatus;
    }
}
