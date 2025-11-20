using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : ActivateableObject
{
    public Transform EndPosition;
    public GameObject Platform;
    public float SelfResetInterval;
    private Vector3 _a, _b;
    private Vector3 _defaultPosition, _endPosition;
    private PeriodicAction _resetCountdown;
    private bool _countDown;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _resetCountdown= new PeriodicAction(SelfResetInterval, ResetPosition, null);
        _defaultPosition = Platform.transform.position;
        _endPosition = EndPosition.position;
        RegisterObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (_countDown) 
        {
            _resetCountdown.ProcessAction(Time.deltaTime);
        }
        if (_actuating)
        {
            if (SharedFunctions.ProcessVector3Lerp(ref _actuationElapsed, _actuationTime, _a, _b, Platform.transform))
            {
                _actuating = false;
                if(transform.position == _endPosition)
                {
                    _countDown = true;
                }
            }
        }
    }
    private void ResetPosition()
    {
        _currentState = 0;
        ApplyStateChange(false);
    }
    protected override void ApplyStateChange(bool force)
    {
        _countDown = false;
        _actuating = true;
        _a = _currentState == 0 ? _endPosition : _defaultPosition;
        _b = _currentState == 0 ? _defaultPosition : _endPosition;
        if (ActuationAudio.clip != null)
        {
            ActuationAudio.Play();
        }
    }
}
