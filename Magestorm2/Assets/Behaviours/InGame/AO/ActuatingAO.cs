using UnityEngine;
using UnityEngine.Rendering;

public class ActuatingAO : ActivateableObject
{
    public GameObject ActuatingObject;
    public byte ActuationSpeed; // The distance between the start and end positions, divided by the ActuationSpeed, results in the time it will take to transition from one state to the next.
    protected Vector3 _default, _end;
    protected Vector3 _a, _b;
   
    protected bool _actuating;
    protected float _actuationTime;
    protected float _actuationElapsed;
    public bool Interruptible;

    protected virtual void Start()
    {
        _a = _default;
        _b = _end;
        _actuating = false;
        _actuationElapsed = 0;
        _actuationTime = Vector3.Distance(_a, _b) / ActuationSpeed;
    }

    protected override void ApplyStateChange(bool force)
    {
        _a = _currentState == 0 ? _end : _default;
        _b = _currentState == 0 ? _default : _end;
        base.ApplyStateChange(force);
        if(_actuationElapsed> 0.05f)
        {
            OnInterrupt();
        }
        _actuating = true;
    }
    protected virtual void OnInterrupt()
    {
        Debug.Log("Pre-interrupt: " + _actuationElapsed + " " + _actuationTime);
        _actuationElapsed = (1 - _actuationElapsed / _actuationTime) * _actuationTime;
        Debug.Log("Post-interrupt: " + _actuationElapsed);
    }
    public override void StateChangeRequest()
    {
        if(!_actuating || Interruptible)
        {
            base.StateChangeRequest();
        }
    }
}
