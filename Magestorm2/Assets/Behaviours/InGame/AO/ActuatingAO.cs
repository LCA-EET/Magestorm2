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

    protected override void Start()
    {
        _actuating = false;
        _actuationElapsed = 0;
        _actuationTime = Vector3.Distance(_a, _b) / ActuationSpeed;
        base.Start();
    }
    protected override void ApplyStateChange(bool force)
    {
        base.ApplyStateChange(force);
        if(_actuationElapsed > 0.05f) // for further investigation. It is unclear how _actuationElapsed is being updated when _actuating is false.
        {
            OnInterrupt();
            // the actuation was interrupted.
        }
        _actuating = true;
        //
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
