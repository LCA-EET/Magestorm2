using UnityEngine;

public class ActivateableObject : MonoBehaviour
{
    public byte ObjectKey;
    public byte NumStates = 2;
    
    public AudioSource ActuationAudio;
    protected byte _currentState = 0;
    protected bool _actuating = false;
    protected float _actuationTime = 1.0f;
    protected float _actuationElapsed = 0.0f;
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
            if(_timeRemainingBeforeCanReactivate <= 0)
            {
                Debug.Log("AO reactivation timer elapsed.");
            }
            else
            {
                Debug.Log("AO time remaining: " + _timeRemainingBeforeCanReactivate);
            }
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
            if (_currentState < (NumStates-1))
            {
                newState = (byte)(_currentState + 1);
            }
            Debug.Log("Object State Change Packet Sent");
            //_timeRemainingBeforeCanReactivate = _activationInterval;
            Match.Send(InGame_Packets.ChangedObjectStatePacket(ObjectKey, newState));
        }
    }
    protected virtual void ApplyStateChange(bool force)
    {

    }
    public void StatusChanged(byte newStatus, bool force)
    {
        _currentState = newStatus;
        ApplyStateChange(force);
    }
}
