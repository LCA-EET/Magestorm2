using UnityEngine;

public class ActivateableObject : MonoBehaviour
{
    public byte ObjectKey;
    public byte NumStates = 2;
    private byte _currentState = 0;
    private const float _activationInterval = 3;
    private float _timeRemainingBeforeCanReactivate = 0;

    private void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RegisterObject();
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
        Match.RegisterActivateableObject(this);
    }

    public void PlayerChangedStatus()
    {
        if(_timeRemainingBeforeCanReactivate <= 0)
        {
            if(_currentState == (NumStates - 1))
            {
                _currentState = 0;
            }
            else
            {
                _currentState++;
            }
            Debug.Log("Object State Change Packet Sent");
            _timeRemainingBeforeCanReactivate = _activationInterval;
            Match.Send(InGame_Packets.ChangedObjectStatePacket(ObjectKey, _currentState));
        }
    }
    public void StatusChanged(byte newStatus)
    {
        _currentState = newStatus;
    }
}
