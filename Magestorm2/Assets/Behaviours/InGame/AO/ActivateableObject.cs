using System;
using UnityEngine;

public class ActivateableObject : MonoBehaviour, IComparable<ActivateableObject>
{
    protected byte _objectKey;
    public byte NumStates = 2;
    public AudioSource ActivationAudio;
    public byte ReactivationInterval; // The minimum amount of time that must elapse, in seconds, before the AO can be reactivated.
    public byte SelfResetInterval = 0; // The time that must elapse before the AO resets to its default state, after completing the transition to a non-default state.

    protected byte _currentState = 0;
    protected PeriodicAction _resetCountdownPA;
    protected PeriodicAction _reactivationCountdownPA;
    protected bool _resetCountDown;
    protected bool _readyToActivate;

    public byte ObjectKey
    {
        get { return _objectKey; }
    }

    private void Awake()
    {
        _readyToActivate = true;
        if(SelfResetInterval > 0)
        {
            _resetCountdownPA = new PeriodicAction(SelfResetInterval, ResetToDefault, null);
        }
        if (ReactivationInterval > 0)
        {
            _reactivationCountdownPA = new PeriodicAction(ReactivationInterval, MakeReadyForReactivation, null);
        }
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        if (!_readyToActivate)
        {
            _reactivationCountdownPA.ProcessAction(Time.deltaTime);
        }
    }

    public void RegisterObject(byte objectKey)
    {
        _objectKey = objectKey;
        Debug.Log("Registered AO with key: " + _objectKey);
        Match.RegisterActivateableObject(this);
    }

    public virtual void StateChangeRequest()
    {
        if(_readyToActivate)
        {
            byte newState = 0;
            if (_currentState < (NumStates-1))
            {
                newState = (byte)(_currentState + 1);
            }
            Debug.Log("Object State Change Packet Sent");
            Match.Send(InGame_Packets.ChangedObjectStatePacket(_objectKey, newState, SelfResetInterval));
        }
    }
    protected virtual void ApplyStateChange(bool force)
    {
        _resetCountDown = false;
        if (ReactivationInterval > 0)
        {
            _readyToActivate = false;
        }
        if (ActivationAudio.clip != null)
        {
            ActivationAudio.Play();
        }
    }
    public void StatusChanged(byte newStatus, bool force)
    {
        _currentState = newStatus;
        ApplyStateChange(force);
    }
    private void ResetToDefault()
    {
        StatusChanged(0, false);
    }
    private void MakeReadyForReactivation()
    {
        _readyToActivate = true;
    }

    public int CompareTo(ActivateableObject other)
    {
        return SharedFunctions.CompareVectors(transform.position, other.transform.position);
    }
}
