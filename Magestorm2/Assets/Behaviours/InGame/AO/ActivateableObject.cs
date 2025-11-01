using UnityEngine;

public class ActivateableObject : MonoBehaviour
{
    public byte ObjectKey;
    public byte NumStates = 2;
    protected byte _currentState = 0;
    private const float _activationInterval = 1;
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

    public void StateChangeRequest()
    {
        if(_timeRemainingBeforeCanReactivate <= 0)
        {
            byte newState = 0;
            if (_currentState < NumStates)
            {
                newState = (byte)(_currentState + 1);
            }
            Debug.Log("Object State Change Packet Sent");
            _timeRemainingBeforeCanReactivate = _activationInterval;
            Match.Send(InGame_Packets.ChangedObjectStatePacket(ObjectKey, newState));
        }
    }
    protected virtual void ApplyStateChange()
    {

    }
    public void StatusChanged(byte newStatus)
    {
        _currentState = newStatus;
        ApplyStateChange();
    }
}
