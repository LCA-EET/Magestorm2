using UnityEngine;

public class TranslatingAO : ActuatingAO
{
    public GameObject EndPosition;
    private Vector3 _priorPosition;
    public Trigger PlatformTrigger;
    protected override void Start()
    {
        base.Start();
        _default = ActuatingObject.transform.position;
        _priorPosition = _default;
        _end = EndPosition.transform.position;
        _a = _default;
        _b = _end;

        _actuationTime = Vector3.Distance(_a, _b) / ActuationSpeed;
    }
    protected override void Update()
    {
        base.Update();
        if (_resetCountDown && (_resetCountdownPA != null))
        {
            _resetCountdownPA.ProcessAction(Time.deltaTime);
        }
        if (_actuating)
        {
            Vector3 calculatedLerp = SharedFunctions.CalculateVector3Lerp(ref _actuationElapsed, _actuationTime, _a, _b);
            Vector3 delta = calculatedLerp - _priorPosition;
            _priorPosition = calculatedLerp;
            if (PlatformTrigger != null)
            {
                if (PlatformTrigger.HasEntered())
                {
                    Debug.Log("Adjusting player position.");
                    ComponentRegister.PC.ApplyPositionDelta(delta);
                }
            }
            SharedFunctions.ApplyVector3Lerp(calculatedLerp, ActuatingObject.transform, false, true);
            if (_actuationElapsed == 0) // this is 0 when it is reset by the CalculateVector3Lerp function above. It is reset when elapsed >= actuationTime.
            {
                _actuating = false;
                if (ActuatingObject.transform.position == _end)
                {
                    _resetCountDown = true;
                }
            }
        }
    }
    protected override void ApplyStateChange(bool force)
    {
        if (force)
        {
            ActuatingObject.transform.position = _end;
        }
        else
        {
            base.ApplyStateChange(force);
            _a = _currentState == 0 ? _end : _default;
            _b = _currentState == 0 ? _default : _end;
        }
            
    }
}
