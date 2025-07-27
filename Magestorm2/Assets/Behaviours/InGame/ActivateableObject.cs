using UnityEngine;

public class ActivateableObject : MonoBehaviour
{
    public byte ObjectKey;
    protected byte _objectStatus;
    private const float _activationInterval = 3000;
    private float _timeRemainingBeforeCanReactivate = 0;

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
        if(_timeRemainingBeforeCanReactivate > 0)
        {
            _timeRemainingBeforeCanReactivate -= Time.deltaTime;
        }
    }

    protected void RegisterObject()
    {

    }

    public void PlayerChangedStatus()
    {
        if(_timeRemainingBeforeCanReactivate <= 0)
        {
            _timeRemainingBeforeCanReactivate = _activationInterval;
            Match.Send(InGame_Packets.ChangedObjectStatePacket(ObjectKey, _objectStatus));
        }
    }
    public void StatusChanged(byte newStatus)
    {
        _objectStatus = newStatus;
    }
}
